using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application;
using InSite.Application.Organizations.Read;
using InSite.Application.Organizations.Write;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class OrganizationStore : IOrganizationStore
    {
        private readonly IJsonSerializer _serializer;

        public OrganizationStore(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        internal static InternalDbContext CreateContext() => new InternalDbContext(true);

        #region Static

        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        internal static void SetupGlossary(Guid glossary, Guid organizationId)
        {
            _commander.Send(new ModifyOrganizationGlossary(organizationId, glossary));

            OrganizationSearch.Refresh(organizationId);
        }

        public static void Insert(OrganizationState organization)
        {
            var organizationId = organization.OrganizationIdentifier;
            var commands = OrganizationCommandCreator.Create(organization, true);

            _commander.Send(new RunCommands(organizationId, commands.ToArray()));

            OrganizationSearch.Refresh();
        }

        public static void Update(OrganizationState organization)
        {
            var organizationId = organization.OrganizationIdentifier;

            var commands = OrganizationCommandCreator.Create(organization, false);

            _commander.Send(new RunCommands(organizationId, commands.ToArray()));

            OrganizationSearch.Refresh(organizationId);
        }

        public static void Open(Guid organizationId)
        {
            _commander.Send(new OpenOrganization(organizationId));

            OrganizationSearch.Refresh(organizationId);
        }

        public static void Close(Guid organizationId)
        {
            _commander.Send(new CloseOrganization(organizationId));

            OrganizationSearch.Refresh(organizationId);
        }

        #endregion

        #region Transactions

        private enum EntityOperation { Insert, Modify, Remove, Ignore }

        private interface IEntityState
        {
            EntityOperation Operation { get; }
            object EntityObject { get; }
        }

        private class EntityState<T> : IEntityState
        {
            public EntityOperation Operation { get; }
            public T Entity { get; }

            public object EntityObject => Entity;

            public EntityState(EntityOperation operation, T entity)
            {
                Operation = operation;
                Entity = entity;
            }
        }

        private class OrganizationEntityState : EntityState<QOrganization>
        {
            public OrganizationState State { get; }

            public OrganizationEntityState(EntityOperation operation, QOrganization entity, OrganizationState state)
                : base(operation, entity)
            {
                State = state;
            }
        }

        private class TransactionState
        {
            public Guid AggregateId { get; }

            public OrganizationEntityState Organization { get; set; }

            public TransactionState(Guid aggregateId)
            {
                AggregateId = aggregateId;
            }

            public OrganizationEntityState LoadOrganization(OrganizationState state)
            {
                if (Organization == null)
                {
                    using (var db = CreateContext())
                    {
                        var entity = db.QOrganizations.Where(x => x.OrganizationIdentifier == AggregateId).FirstOrDefault();
                        Organization = new OrganizationEntityState(EntityOperation.Modify, entity, state);
                    }
                }

                return Organization;
            }
        }

        private readonly ConcurrentDictionary<Guid, TransactionState> _transactions = new ConcurrentDictionary<Guid, TransactionState>();

        public Guid StartTransaction(Guid aggregateId)
        {
            var transactionId = Guid.NewGuid();

            if (!_transactions.TryAdd(transactionId, new TransactionState(aggregateId)))
                throw new ArgumentException($"TransactionId is already used: {transactionId}");

            return transactionId;
        }

        public void CancelTransaction(Guid transactionId)
        {
            _transactions.TryRemove(transactionId, out _);
        }

        public void CommitTransaction(Guid transactionId)
        {
            if (!_transactions.TryGetValue(transactionId, out var transaction))
                throw new ArgumentException($"Transaction does not exist: {transactionId}");

            using (var db = CreateContext())
            {
                if (transaction.Organization == null || !RemoveOrganization(db, transaction.Organization))
                {
                    if (transaction.Organization != null)
                    {
                        transaction.Organization.Entity.OrganizationData = _serializer.Serialize(transaction.Organization.State);

                        SaveEntity(db, transaction.Organization);
                    }
                }

                db.SaveChanges();
            }

            _transactions.TryRemove(transactionId, out _);
        }

        private TransactionState GetTransaction(Guid transactionId, Guid aggregateId)
        {
            var transaction = _transactions[transactionId];

            if (transaction.AggregateId != aggregateId)
                throw new ArgumentException($"Transaction ${transactionId} stores organization ${transaction.AggregateId}, but was requested ${aggregateId}");

            return transaction;
        }

        private void SaveEntity(InternalDbContext db, IEntityState entityState)
        {
            if (entityState.Operation == EntityOperation.Ignore)
                return;

            if (entityState.EntityObject == null)
                throw new ArgumentNullException("entityState.EntityObject");

            switch (entityState.Operation)
            {
                case EntityOperation.Insert:
                    db.Entry(entityState.EntityObject).State = EntityState.Added;
                    break;
                case EntityOperation.Modify:
                    db.Entry(entityState.EntityObject).State = EntityState.Modified;
                    break;
                case EntityOperation.Remove:
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation: {entityState.Operation}");
            }
        }

        private bool RemoveOrganization(InternalDbContext db, OrganizationEntityState state)
        {
            if (state.Operation != EntityOperation.Remove)
                return false;

            db.QOrganizations.Attach(state.Entity);
            db.Entry(state.Entity).State = EntityState.Deleted;

            return true;
        }

        #endregion

        #region Organization

        public void Update(OrganizationAccountSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationAchievementSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationAdministratorModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.AdministratorUserIdentifier = state.AdministratorUserIdentifier;
        });

        public void Update(OrganizationAnnouncementModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationAssessmentSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationAutomaticCompetencyExpirationModified e) => UpdateOrganization(e, (entity, state) =>
        {
            var settings = state.PlatformCustomization.AutomaticCompetencyExpiration;
            entity.CompetencyAutoExpirationMode = settings.Type.GetName();
            entity.CompetencyAutoExpirationMonth = settings.Month;
            entity.CompetencyAutoExpirationDay = settings.Day;
        });

        public void Update(OrganizationClosed e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.AccountClosed = state.AccountClosed;
            entity.AccountStatus = state.AccountStatus.GetName();
        });

        public void Update(OrganizationContactSettingsModified e)
        {
            UpdateOrganization(e, (entity, state) =>
            {
                entity.PersonFullNamePolicy = e.Contacts?.FullNamePolicy;
            });
        }

        public void Insert(OrganizationCreated e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = _transactions[transactionId];
            if (transaction.Organization != null)
                throw new ArgumentException("Invalid change: OrganizationCreated");

            var state = (OrganizationState)e.AggregateState;
            var entity = new QOrganization
            {
                OrganizationIdentifier = e.AggregateIdentifier,
                AccountOpened = state.AccountOpened,
                AccountStatus = state.AccountStatus.GetName(),
                TimeZone = state.TimeZone.Id,
                CompetencyAutoExpirationMode = state.PlatformCustomization.AutomaticCompetencyExpiration.Type.GetName(),
            };

            transaction.Organization = new OrganizationEntityState(EntityOperation.Insert, entity, state);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Delete(OrganizationDeleted e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier);

            transaction.Organization = new OrganizationEntityState(EntityOperation.Remove, new QOrganization
            {
                OrganizationIdentifier = e.AggregateIdentifier
            }, null);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Update(OrganizationDescriptionModified e) => UpdateOrganization(e, (entity, state) =>
        {
            var decription = state.CompanyDescription;
            entity.CompanySize = decription.CompanySize.GetName();
            entity.CompanySummary = decription.CompanySummary.MaxLength(900);
            entity.CompanyTitle = decription.LegalName.MaxLength(100);
        });

        public void Update(OrganizationEventSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationFieldsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationGlossaryModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.GlossaryIdentifier = state.GlossaryIdentifier;
        });

        public void Update(OrganizationGradebookSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationIdentificationModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.OrganizationCode = state.OrganizationCode;
            entity.CompanyName = state.CompanyName.MaxLength(50);
            entity.CompanyDomain = state.CompanyDomain.MaxLength(50);
        });

        public void Update(OrganizationIntegrationSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationIssueSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationLocalizationModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.TimeZone = state.TimeZone.Id;
        });

        public void Update(OrganizationLocationModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationNCSHASettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationOpened e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.AccountOpened = state.AccountOpened;
            entity.AccountClosed = state.AccountClosed;
            entity.AccountStatus = state.AccountStatus.GetName();
        });

        public void Update(OrganizationParentModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.ParentOrganizationIdentifier = state.ParentOrganizationIdentifier;
        });

        public void Update(OrganizationPlatformSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationPlatformUrlModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.OrganizationLogoUrl = state.PlatformCustomization.PlatformUrl.Logo.MaxLength(500);
        });

        public void Update(OrganizationPortalSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationRegistrationSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationSalesSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationSecretModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationSignInModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationSiteSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationStandardContentLabelsModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.StandardContentLabels = state.StandardContentLabels;
        });

        public void Update(OrganizationStandardSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationSurveySettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationTypeModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationUploadSettingsModified e) => UpdateOrganization(e, (entity, state) => { });

        public void Update(OrganizationUrlsModified e) => UpdateOrganization(e, (entity, state) =>
        {
            entity.CompanyWebSiteUrl = state.PlatformCustomization.TenantUrl.WebSite.MaxLength(500);
        });

        private void UpdateOrganization(Change e, Action<QOrganization, OrganizationState> action)
        {
            var isTransaction = e.ChangeTransactionId.HasValue;
            var state = (OrganizationState)e.AggregateState;
            var transactionId = isTransaction ? e.ChangeTransactionId.Value : StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier);
            var entityState = transaction.LoadOrganization(state);

            if (entityState.Entity != null)
                action?.Invoke(entityState.Entity, state);

            if (!isTransaction)
                CommitTransaction(transactionId);
        }

        #endregion
    }
}