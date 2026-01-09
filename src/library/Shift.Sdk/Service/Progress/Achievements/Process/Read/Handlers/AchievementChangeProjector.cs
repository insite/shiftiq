using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Application.Logs.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the projector for Achievement events.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class AchievementChangeProjector
    {
        private static readonly HashSet<string> _obsoleteChanges = new HashSet<string>
        {
            "TradeworkNumberIsRequiredChanged",
            "MultipleRegistrationsAllowed",
            "MultipleRegistrationsDisallowed",
            "AchievementWebhookChanged"
        };

        private readonly IAchievementStore _store;
        private readonly IAggregateSearch _aggregates;

        public AchievementChangeProjector(IChangeQueue publisher, IChangeStore changeStore, IAggregateSearch aggregates, IAchievementStore store)
        {
            _store = store;
            _aggregates = aggregates;

            publisher.Subscribe<AchievementCreated>(Handle);
            publisher.Subscribe<AchievementDescribed>(Handle);
            publisher.Subscribe<CertificateLayoutChanged>(Handle);
            publisher.Subscribe<AchievementExpiryChanged>(Handle);
            publisher.Subscribe<AchievementTenantChanged>(Handle);
            publisher.Subscribe<AchievementTypeChanged>(Handle);
            publisher.Subscribe<AchievementUnlocked>(Handle);
            publisher.Subscribe<AchievementLocked>(Handle);
            publisher.Subscribe<AchievementDeleted>(Handle);

            publisher.Subscribe<AchievementBadgeImageChanged>(Handle);

            publisher.Subscribe<AchievementReportingDisabled>(Handle);
            publisher.Subscribe<AchievementReportingEnabled>(Handle);

            publisher.Subscribe<AchievementBadgeImageDisabled>(Handle);
            publisher.Subscribe<AchievementBadgeImageEnabled>(Handle);

            publisher.Subscribe<AchievementPrerequisiteAdded>(Handle);
            publisher.Subscribe<AchievementPrerequisiteDeleted>(Handle);

            publisher.Subscribe<CredentialCreated>(Handle);
            publisher.Subscribe<CredentialExpired2>(Handle);
            publisher.Subscribe<CredentialEmployerChanged>(Handle);
            publisher.Subscribe<CredentialGranted3>(Handle);
            publisher.Subscribe<CredentialAuthorityChanged>(Handle);
            publisher.Subscribe<CredentialExpirationChanged>(Handle);
            publisher.Subscribe<CredentialTagged>(Handle);
            publisher.Subscribe<CredentialRevoked2>(Handle);
            publisher.Subscribe<CredentialDeleted2>(Handle);
            publisher.Subscribe<CredentialDescribed2>(Handle);
            publisher.Subscribe<CredentialPublishedOnChain>(Handle);

            publisher.Subscribe<ExpirationReminderRequested2>(Handle);
            publisher.Subscribe<ExpirationReminderDelivered2>(Handle);

            changeStore.RegisterObsoleteChangeTypes(_obsoleteChanges);
        }

        public void Handle(AchievementCreated e)
            => _store.InsertAchievement(e);

        public void Handle(AchievementDescribed e)
            => _store.UpdateAchievement(e);

        public void Handle(CertificateLayoutChanged e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementExpiryChanged e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementPrerequisiteAdded e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementPrerequisiteDeleted e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementTenantChanged e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementTypeChanged e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementBadgeImageChanged e)
             => _store.UpdateAchievement(e);

        public void Handle(AchievementBadgeImageDisabled e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementBadgeImageEnabled e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementUnlocked e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementLocked e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementReportingDisabled e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementReportingEnabled e)
            => _store.UpdateAchievement(e);

        public void Handle(AchievementDeleted e)
            => _store.DeleteAchievement(e);

        public void Handle(CredentialCreated e)
            => _store.InsertCredential(e, CredentialStatus.Pending);

        public void Handle(CredentialExpired2 e)
            => _store.UpdateCredential(e, CredentialStatus.Expired);

        public void Handle(CredentialEmployerChanged e)
            => _store.UpdateCredential(e);

        public void Handle(CredentialGranted3 e)
            => _store.UpdateCredential(e, CredentialStatus.Valid);

        public void Handle(CredentialAuthorityChanged e)
            => _store.UpdateCredential(e);

        public void Handle(CredentialExpirationChanged e)
            => _store.UpdateCredential(e);

        public void Handle(CredentialTagged e)
            => _store.UpdateCredential(e);

        public void Handle(CredentialRevoked2 e)
            => _store.UpdateCredential(e, CredentialStatus.Pending);

        public void Handle(CredentialDeleted2 e)
            => _store.DeleteCredential(e);

        public void Handle(CredentialDescribed2 e)
            => _store.UpdateCredential(e);

        public void Handle(CredentialPublishedOnChain e) { }

        public void Handle(ExpirationReminderRequested2 e)
            => _store.UpdateCredential(e);

        public void Handle(ExpirationReminderDelivered2 e)
            => _store.UpdateCredential(e);

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress)
        {
            // Clear all the existing data in the query store for this projection.
            _store.DeleteAll();

            ReplayAll("Achievement", store, progress);
            ReplayAll("Credential", store, progress);
        }

        private void ReplayAll(string type, IChangeStore store, Action<string, int, int, Guid> progress)
        {
            // Get the list of aggregates for the specified organization.
            var aggregates = _aggregates.GetByType(type, null);

            for (var i = 0; i < aggregates.Length; i++)
            {
                var aggregate = aggregates[i];

                progress(type, i + 1, aggregates.Length, aggregate.AggregateIdentifier);

                // Get the subset of events for which this projection is a subscriber. 
                var changes = store.GetChanges(type, aggregate.AggregateIdentifier, null);

                // Handle each of the events in the order they occurred.
                for (var j = 0; j < changes.Length; j++)
                {
                    var e = changes[j];

                    var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                    if (handler == null)
                        throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                    handler.Invoke(this, new[] { e });
                }
            }
        }

        public void Replay(IChangeStore store, Guid aggregate, int version = 0)
        {
            // Clear all the existing data in the query store for this projection.
            _store.Delete(aggregate);

            // Get the list of aggregates for the specified organization.
            var changes = store.GetChanges(aggregate, version);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }

        public void ReplayCredential(IChangeStore store, Action<string, int, int, Guid> progress, Guid id)
        {
            _store.DeleteCredential(id);
            Replay(store, id);
        }
    }
}
