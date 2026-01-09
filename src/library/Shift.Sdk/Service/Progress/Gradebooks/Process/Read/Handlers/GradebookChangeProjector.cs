using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the projector for Gradebook events.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class GradebookChangeProjector
    {
        private readonly IRecordStore _store;

        public GradebookChangeProjector(IChangeQueue publisher, IChangeStore changeStore, IRecordStore store)
        {
            _store = store;

            publisher.Subscribe<GradebookCalculated>(Handle);
            publisher.Subscribe<GradebookCreated>(Handle);
            publisher.Subscribe<GradebookDeleted>(Handle);
            publisher.Subscribe<GradebookLocked>(Handle);
            publisher.Subscribe<GradebookUnlocked>(Handle);
            publisher.Subscribe<GradebookReferenced>(Handle);
            publisher.Subscribe<GradebookRenamed>(Handle);
            publisher.Subscribe<GradebookAchievementChanged>(Handle);
            publisher.Subscribe<GradebookEventChanged>(Handle);
            publisher.Subscribe<GradebookEventAdded>(Handle);
            publisher.Subscribe<GradebookEventRemoved>(Handle);
            publisher.Subscribe<GradebookPeriodChanged>(Handle);
            publisher.Subscribe<GradebookTypeChanged>(Handle);
            publisher.Subscribe<GradebookWarningAdded>(Handle);

            publisher.Subscribe<GradeItemAdded>(Handle);
            publisher.Subscribe<GradeItemAchievementChanged>(Handle);
            publisher.Subscribe<GradeItemCalculationChanged>(Handle);
            publisher.Subscribe<GradeItemChanged>(Handle);
            publisher.Subscribe<GradeItemCompetenciesChanged>(Handle);
            publisher.Subscribe<GradeItemHookChanged>(Handle);
            publisher.Subscribe<GradeItemMaxPointsChanged>(Handle);
            publisher.Subscribe<GradeItemNotificationsChanged>(Handle);
            publisher.Subscribe<GradeItemPassPercentChanged>(Handle);
            publisher.Subscribe<GradeItemReferenced>(Handle);
            publisher.Subscribe<GradeItemDeleted>(Handle);
            publisher.Subscribe<GradeItemReordered>(Handle);

            publisher.Subscribe<EnrollmentStarted>(Handle);
            publisher.Subscribe<EnrollmentRestarted>(Handle);
            publisher.Subscribe<GradebookUserNoted>(Handle);
            publisher.Subscribe<GradebookUserDeleted>(Handle);
            publisher.Subscribe<GradebookUserPeriodChanged>(Handle);

            publisher.Subscribe<GradebookValidationAdded>(Handle);
            publisher.Subscribe<GradebookValidationChanged>(Handle);

            publisher.Subscribe<ProgressAdded>(Handle);
            publisher.Subscribe<ProgressCommentChanged>(Handle);
            publisher.Subscribe<ProgressCompleted2>(Handle);
            publisher.Subscribe<ProgressDeleted>(Handle);
            publisher.Subscribe<ProgressHidden>(Handle);
            publisher.Subscribe<ProgressIncompleted>(Handle);
            publisher.Subscribe<ProgressLocked>(Handle);
            publisher.Subscribe<ProgressNumberChanged>(Handle);
            publisher.Subscribe<ProgressPercentChanged>(Handle);
            publisher.Subscribe<ProgressPointsChanged>(Handle);
            publisher.Subscribe<ProgressPublished>(Handle);
            publisher.Subscribe<ProgressShowed>(Handle);
            publisher.Subscribe<ProgressStarted>(Handle);
            publisher.Subscribe<ProgressTextChanged>(Handle);
            publisher.Subscribe<ProgressUnlocked>(Handle);
            publisher.Subscribe<ProgressIgnored>(Handle);

            changeStore.RegisterObsoleteChangeTypes(new[] { ProgressCompleted2.ObsoleteChangeType });
        }

        public void Handle(GradebookCalculated c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookCreated c)
            => _store.InsertRecord(c);

        public void Handle(GradebookDeleted c)
            => _store.DeleteRecord(c);

        public void Handle(GradebookLocked c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookUnlocked c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookReferenced c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookRenamed c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookAchievementChanged c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookEventChanged c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookEventAdded c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookEventRemoved c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookPeriodChanged c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookTypeChanged c)
            => _store.UpdateRecord(c);

        public void Handle(GradebookWarningAdded c)
            => _store.UpdateRecord(c);

        public void Handle(GradeItemAdded c)
            => _store.InsertItem(c);

        public void Handle(GradeItemAchievementChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemCalculationChanged c) { }

        public void Handle(GradeItemChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemCompetenciesChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemHookChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemMaxPointsChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemNotificationsChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemPassPercentChanged c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemReferenced c)
            => _store.UpdateItem(c);

        public void Handle(GradeItemDeleted c)
            => _store.DeleteItem(c);

        public void Handle(GradeItemReordered c)
            => _store.ReorderItems(c);

        public void Handle(EnrollmentStarted c)
            => _store.InsertEnrollment(c);

        public void Handle(EnrollmentRestarted c)
            => _store.UpdateEnrollment(c);

        public void Handle(GradebookUserNoted c)
            => _store.UpdateEnrollment(c);

        public void Handle(GradebookUserDeleted c)
            => _store.DeleteEnrollment(c);

        public void Handle(GradebookUserPeriodChanged c)
            => _store.UpdateEnrollment(c);

        public void Handle(GradebookValidationAdded c)
            => _store.InsertValidation(c);

        public void Handle(GradebookValidationChanged c)
            => _store.UpdateValidation(c);

        public void Handle(ProgressAdded c)
            => _store.InsertProgress(c);

        public void Handle(ProgressCommentChanged c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressCompleted2 c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressDeleted c)
            => _store.DeleteProgress(c);

        public void Handle(ProgressHidden c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressIncompleted c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressLocked c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressNumberChanged c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressPercentChanged c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressPointsChanged c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressPublished c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressShowed c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressStarted c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressTextChanged c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressUnlocked c)
            => _store.UpdateProgress(c);

        public void Handle(ProgressIgnored c)
            => _store.UpdateProgress(c);

        public void ReplayOne(IChangeStore store, Action<string, int, int, Guid> progress, Guid id)
        {
            _store.DeleteOne(id);
            ReplayChanges("Gradebook", store, progress, id);
        }

        public void ReplayAll(IChangeStore store, Action<string, int, int, Guid> progress)
        {
            _store.DeleteAll();
            ReplayChanges("Gradebook", store, progress);
        }

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ProgressCompleted2.ObsoleteChangeType)
                return;

            var v2 = ProgressCompleted2.Upgrade(e);

            Handle(v2);
        }

        private void ReplayChanges(string aggregateType, IChangeStore store, Action<string, int, int, Guid> progress, Guid id)
        {
            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges(aggregateType, id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress(aggregateType, i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() })
                    ?? throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }

        private void ReplayChanges(string aggregateType, IChangeStore store, Action<string, int, int, Guid> progress)
        {
            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges(aggregateType, null, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress(aggregateType, i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() })
                    ?? throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
