using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Records.Write
{
    public class JournalSetupCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IJournalSearch _journalSearch;

        public JournalSetupCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IJournalSearch journalSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _journalSearch = journalSearch;

            commander.Subscribe<AddCompetencyRequirement>(Handle);
            commander.Subscribe<ChangeCompetencyRequirement>(Handle);
            commander.Subscribe<DeleteCompetencyRequirement>(Handle);
            commander.Subscribe<ChangeJournalSetupAchievement>(Handle);
            commander.Subscribe<ChangeJournalSetupContent>(Handle);
            commander.Subscribe<ChangeLockUnlockJournalSetup>(Handle);
            commander.Subscribe<ChangeJournalSetupEvent>(Handle);
            commander.Subscribe<CreateJournalSetup>(Handle);
            commander.Subscribe<DeleteJournalSetup>(Handle);
            commander.Subscribe<AddJournalSetupField>(Handle);
            commander.Subscribe<ChangeJournalSetupField>(Handle);
            commander.Subscribe<ChangeJournalSetupFieldContent>(Handle);
            commander.Subscribe<DeleteJournalSetupField>(Handle);
            commander.Subscribe<ChangeJournalSetupFramework>(Handle);
            commander.Subscribe<ChangeJournalSetupIsValidationRequired>(Handle);
            commander.Subscribe<AllowLogbookDownload>(Handle);
            commander.Subscribe<DisallowLogbookDownload>(Handle);
            commander.Subscribe<ModifyJournalSetupAreaHours>(Handle);
            commander.Subscribe<ChangeJournalSetupMessages>(Handle);
            commander.Subscribe<RenameJournalSetup>(Handle);
            commander.Subscribe<AddJournalSetupUser>(Handle);
            commander.Subscribe<DeleteJournalSetupUser>(Handle);
            commander.Subscribe<CreateJournalSetupGroup>(Handle);
            commander.Subscribe<RemoveJournalSetupGroup>(Handle);
            commander.Subscribe<ReorderJournalSetupFields>(Handle);
        }

        private void Commit(JournalSetupAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddCompetencyRequirement c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.FindCompetencyRequirement(c.Competency) != null)
                    return;

                aggregate.AddCompetencyRequirement(c.Competency, c.Hours, c.JournalItems, c.SkillRating, c.IncludeHoursToArea);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeCompetencyRequirement c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var competency = aggregate.Data.FindCompetencyRequirement(c.Competency);
                if (competency == null)
                    throw new JournalSetupException($"Competency {c.Competency} is not found in journal setup {c.AggregateIdentifier}");

                aggregate.ChangeCompetencyRequirement(c.Competency, c.Hours, c.JournalItems, c.SkillRating, c.IncludeHoursToArea);

                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteCompetencyRequirement c)
        {
            if (_journalSearch.ExperienceExists(new QExperienceFilter { JournalSetupIdentifier = c.AggregateIdentifier, CompetencyStandardIdentifier = c.Competency }))
                throw new JournalSetupException($"Competency {c.Competency} of journal setup {c.AggregateIdentifier} is used in journals");

            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var competency = aggregate.Data.FindCompetencyRequirement(c.Competency);
                if (competency == null)
                    throw new JournalSetupException($"Competency {c.Competency} is not found in journal setup {c.AggregateIdentifier}");

                aggregate.DeleteCompetencyRequirement(c.Competency);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeJournalSetupAchievement c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Achievement != c.Achievement)
                {
                    aggregate.ChangeAchievement(c.Achievement);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeJournalSetupContent c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (!ContentContainer.IsEqual(aggregate.Data.Content, c.Content))
                {
                    aggregate.ChangeContent(c.Content);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeLockUnlockJournalSetup c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.JournalSetupLocked != c.JournalSetupLocked)
                {
                    aggregate.ChangeLockUnlockJournalSetup(c.JournalSetupLocked);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeJournalSetupEvent c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Event != c.Event)
                {
                    aggregate.ChangeEvent(c.Event);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(CreateJournalSetup c)
        {
            var aggregate = new JournalSetupAggregate { AggregateIdentifier = c.AggregateIdentifier };

            aggregate.Create(c.Tenant, c.Name);

            Commit(aggregate, c);
        }

        public void Handle(DeleteJournalSetup c)
        {
            if (_journalSearch.JournalExists(new QJournalFilter { JournalSetupIdentifier = c.AggregateIdentifier }))
                throw new JournalSetupException($"Journal setup {c.AggregateIdentifier} has referenced journals and cannot be deleted");

            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Delete();

                Commit(aggregate, c);
            });
        }

        public void Handle(AddJournalSetupField c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var field = aggregate.Data.FindField(c.Field);
                if (field != null)
                    throw new JournalSetupException($"Journal field {c.Field} is already exist in journal setup {c.AggregateIdentifier}");

                if (aggregate.Data.FindField(c.Type) != null)
                    throw new JournalSetupException($"Field '{c.Type}' is already exist in journal setup {aggregate.AggregateIdentifier}");

                aggregate.AddJournalSetupField(c.Field, c.Type, c.Sequence, c.IsRequired);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeJournalSetupField c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var field = aggregate.Data.FindField(c.Field);
                if (field == null)
                    throw new JournalSetupException($"Journal field {c.Field} is not found in journal setup {c.AggregateIdentifier}");

                if (field.IsRequired != c.IsRequired)
                {
                    aggregate.ChangeJournalSetupField(c.Field, c.IsRequired);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeJournalSetupFieldContent c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var field = aggregate.Data.FindField(c.Field);
                if (field == null)
                    throw new JournalSetupException($"Journal field {c.Field} is not found in journal setup {c.AggregateIdentifier}");

                if (!ContentContainer.IsEqual(field.Content, c.Content))
                {
                    aggregate.ChangeJournalSetupFieldContent(c.Field, c.Content);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(DeleteJournalSetupField c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var field = aggregate.Data.FindField(c.Field);
                if (field == null)
                    throw new JournalSetupException($"Journal field {c.Field} is not found in journal setup {c.AggregateIdentifier}");

                aggregate.DeleteJournalSetupField(c.Field);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeJournalSetupFramework c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Framework == c.Framework)
                    return;

                if (aggregate.Data.CompetencyRequirements != null && aggregate.Data.CompetencyRequirements.Count > 0)
                    throw new JournalSetupException($"Framework for journal setup {aggregate.AggregateIdentifier} cannot be changed while it has assigned competencies");

                aggregate.ChangeFramework(c.Framework);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeJournalSetupIsValidationRequired c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.IsValidationRequired != c.IsValidationRequired)
                {
                    aggregate.ChangeIsValidationRequired(c.IsValidationRequired);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(AllowLogbookDownload c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.AllowLogbookDownload();

                Commit(aggregate, c);
            });

        }

        public void Handle(DisallowLogbookDownload c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.DisallowLogbookDownload();

                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyJournalSetupAreaHours c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ModifyJournalSetupAreaHours(c.Area, c.Hours);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeJournalSetupMessages c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.ValidatorMessage != c.ValidatorMessage
                    || aggregate.Data.LearnerMessage != c.LearnerMessage
                    || aggregate.Data.LearnerAddedMessage != c.LearnerAddedMessage
                    )
                {
                    aggregate.ChangeMessages(c.ValidatorMessage, c.LearnerMessage, c.LearnerAddedMessage);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(RenameJournalSetup c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (!string.Equals(aggregate.Data.Name, c.Name))
                {
                    aggregate.Rename(c.Name);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(AddJournalSetupUser c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.ContainsUser(c.User, c.Role))
                    throw new JournalSetupException($"User '{c.User}' with role '{c.Role}' is already exist in journal setup {aggregate.AggregateIdentifier}");

                aggregate.AddUser(c.User, c.Role);

                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteJournalSetupUser c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.ContainsUser(c.User, c.Role))
                {
                    aggregate.DeleteUser(c.User, c.Role);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(CreateJournalSetupGroup c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.ContainsGroup(c.Group))
                    throw new JournalSetupException($"Group '{c.Group}' already exists in journal setup {aggregate.AggregateIdentifier}");

                aggregate.CreateGroup(c.Group);
                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveJournalSetupGroup c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.ContainsGroup(c.Group))
                {
                    aggregate.RemoveGroup(c.Group);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ReorderJournalSetupFields c)
        {
            _repository.LockAndRun<JournalSetupAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                var isSame = true;

                foreach (var (fieldId, sequence) in c.Fields)
                {
                    var field = data.Fields.Find(x => x.Identifier == fieldId);
                    if (field == null)
                        continue;

                    if (field.Sequence != sequence)
                    {
                        isSame = false;
                        break;
                    }
                }

                if (!isSame)
                {
                    aggregate.ReorderFields(c.Fields);
                    Commit(aggregate, c);
                }
            });
        }
    }
}
