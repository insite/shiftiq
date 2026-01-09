using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Organizations.Read;
using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Write
{
    public class OrganizationCommandReceiver
    {
        private readonly IChangeRepository _repository;
        private readonly IChangeQueue _publisher;
        private readonly IOrganizationStore _organizationStore;

        public OrganizationCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IOrganizationStore organizationStore)
        {
            _repository = repository;
            _publisher = publisher;
            _organizationStore = organizationStore;

            commander.Subscribe<RunCommands>(Handle);

            commander.Subscribe<CloseOrganization>(Handle);
            commander.Subscribe<CreateOrganization>(Handle);
            commander.Subscribe<DeleteOrganization>(Handle);
            commander.Subscribe<ModifyOrganizationAccountSettings>(Handle);
            commander.Subscribe<ModifyOrganizationAchievementSettings>(Handle);
            commander.Subscribe<ModifyOrganizationAdministrator>(Handle);
            commander.Subscribe<ModifyOrganizationAnnouncement>(Handle);
            commander.Subscribe<ModifyOrganizationAssessmentSettings>(Handle);
            commander.Subscribe<ModifyOrganizationAutomaticCompetencyExpiration>(Handle);
            commander.Subscribe<ModifyOrganizationContactSettings>(Handle);
            commander.Subscribe<ModifyOrganizationDescription>(Handle);
            commander.Subscribe<ModifyOrganizationEventSettings>(Handle);
            commander.Subscribe<ModifyOrganizationFields>(Handle);
            commander.Subscribe<ModifyOrganizationGlossary>(Handle);
            commander.Subscribe<ModifyOrganizationGradebookSettings>(Handle);
            commander.Subscribe<ModifyOrganizationIdentification>(Handle);
            commander.Subscribe<ModifyOrganizationIntegrationSettings>(Handle);
            commander.Subscribe<ModifyOrganizationIssueSettings>(Handle);
            commander.Subscribe<ModifyOrganizationLocalization>(Handle);
            commander.Subscribe<ModifyOrganizationLocation>(Handle);
            commander.Subscribe<ModifyOrganizationNCSHASettings>(Handle);
            commander.Subscribe<ModifyOrganizationParent>(Handle);
            commander.Subscribe<ModifyOrganizationPlatformSettings>(Handle);
            commander.Subscribe<ModifyOrganizationPlatformUrl>(Handle);
            commander.Subscribe<ModifyOrganizationPortalSettings>(Handle);
            commander.Subscribe<ModifyOrganizationRegistrationSettings>(Handle);
            commander.Subscribe<ModifyOrganizationSalesSettings>(Handle);
            commander.Subscribe<ModifyOrganizationSecret>(Handle);
            commander.Subscribe<ModifyOrganizationSignIn>(Handle);
            commander.Subscribe<ModifyOrganizationSiteSettings>(Handle);
            commander.Subscribe<ModifyOrganizationStandardContentLabels>(Handle);
            commander.Subscribe<ModifyOrganizationStandardSettings>(Handle);
            commander.Subscribe<ModifyOrganizationSurveySettings>(Handle);
            commander.Subscribe<ModifyOrganizationType>(Handle);
            commander.Subscribe<ModifyOrganizationUploadSettings>(Handle);
            commander.Subscribe<ModifyOrganizationUrls>(Handle);
            commander.Subscribe<OpenOrganization>(Handle);
        }

        private void Handle(RunCommands runCommands)
        {
            if (runCommands.Commands == null || runCommands.Commands.Length == 0)
                return;

            if (runCommands.Commands[0] is IHasAggregate create)
            {
                if (((IHasRun)create).Run(null))
                    RunCommands(create.Aggregate, runCommands, 1);
            }
            else
            {
                _repository.LockAndRun<OrganizationAggregate>(runCommands.AggregateIdentifier, organization =>
                {
                    RunCommands(organization, runCommands, 0);
                });
            }
        }

        private bool RunCommands(OrganizationAggregate aggregate, RunCommands runCommands, int startIndex)
        {
            for (int i = startIndex; i < runCommands.Commands.Length; i++)
            {
                var command = runCommands.Commands[i];

                if (command.AggregateIdentifier != aggregate.AggregateIdentifier)
                    throw new ArgumentException($"The command has wrong AggregateIdentifier: {command.AggregateIdentifier}");

                if (!((IHasRun)command).Run(aggregate))
                    return false;
            }

            var transactionId = _organizationStore.StartTransaction(aggregate.AggregateIdentifier);
            try
            {
                Commit(aggregate, runCommands, transactionId);
                _organizationStore.CommitTransaction(transactionId);
                return true;
            }
            catch
            {
                _organizationStore.CancelTransaction(transactionId);
                throw;
            }
        }

        private void Handle<T>(T c) where T : Command, IHasRun
        {
            if (c is IHasAggregate create)
            {
                if (c.Run(null))
                    Commit(create.Aggregate, c, null);

                return;
            }

            _repository.LockAndRun<OrganizationAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.Run(aggregate))
                    Commit(aggregate, c, null);
            });
        }

        private void Commit(OrganizationAggregate aggregate, ICommand c, Guid? changeTransactionId)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (Change change in changes)
            {
                change.AggregateState = aggregate.State;
                change.ChangeTransactionId = changeTransactionId;
                _publisher.Publish(change);
            }
        }
    }
}