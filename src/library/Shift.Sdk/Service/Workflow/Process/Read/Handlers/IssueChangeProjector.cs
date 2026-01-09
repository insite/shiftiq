using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Issues;

namespace InSite.Application.Issues.Read
{
    public class IssueChangeProjector
    {
        public ICaseStore _store;

        public IssueChangeProjector(IChangeQueue publisher, IChangeStore changeStore, ICaseStore store)
        {
            _store = store;

            publisher.Subscribe<CaseAttachmentAdded>(Handle);
            publisher.Subscribe<CaseAttachmentFileChanged>(Handle);
            publisher.Subscribe<CaseAttachmentFileRenamed>(Handle);
            publisher.Subscribe<CaseAttachmentDeleted>(Handle);
            publisher.Subscribe<CaseFileRequirementAdded>(Handle);
            publisher.Subscribe<CaseFileRequirementModified>(Handle);
            publisher.Subscribe<CaseFileRequirementCompleted>(Handle);
            publisher.Subscribe<CaseFileRequirementDeleted>(Handle);
            publisher.Subscribe<CaseCommentPosted>(Handle);
            publisher.Subscribe<CommentPrivacyChanged>(Handle);
            publisher.Subscribe<CaseCommentDeleted>(Handle);
            publisher.Subscribe<CaseCommentModified>(Handle);
            publisher.Subscribe<CaseClosed>(Handle);
            publisher.Subscribe<CaseConnectedToSurveyResponse>(Handle);
            publisher.Subscribe<CaseDescribed>(Handle);
            publisher.Subscribe<CaseOpened2>(Handle);
            publisher.Subscribe<CaseReopened>(Handle);
            publisher.Subscribe<CaseStatusChanged>(Handle);
            publisher.Subscribe<CaseTitleChanged>(Handle);
            publisher.Subscribe<CaseTypeChanged>(Handle);
            publisher.Subscribe<GroupAssigned>(Handle);
            publisher.Subscribe<GroupUnassigned>(Handle);
            publisher.Subscribe<UserAssigned>(Handle);
            publisher.Subscribe<UserUnassigned>(Handle);
            publisher.Subscribe<CaseDeleted>(Handle);

            changeStore.RegisterObsoleteChangeTypes(new[]
            {
                ObsoleteChangeType.CaseOpened,
                ObsoleteChangeType.CaseAllowLearnerToViewModified
            });
        }

        public void Handle(CaseAttachmentAdded e)
            => _store.Update(e);

        public void Handle(CaseAttachmentFileChanged e)
            => _store.Update(e);

        public void Handle(CaseAttachmentFileRenamed e)
            => _store.Update(e);

        public void Handle(CaseAttachmentDeleted e)
            => _store.Update(e);

        public void Handle(CaseFileRequirementAdded e)
            => _store.Update(e);

        public void Handle(CaseFileRequirementModified e)
            => _store.Update(e);

        public void Handle(CaseFileRequirementCompleted e)
            => _store.Update(e);

        public void Handle(CaseFileRequirementDeleted e)
            => _store.Update(e);

        public void Handle(CaseCommentPosted e)
            => _store.Update(e);

        public void Handle(CommentPrivacyChanged e)
            => _store.Update(e);

        public void Handle(CaseCommentDeleted e)
            => _store.Update(e);

        public void Handle(CaseCommentModified e)
            => _store.Update(e);

        public void Handle(GroupAssigned e)
            => _store.Update(e);

        public void Handle(GroupUnassigned e)
            => _store.Update(e);

        public void Handle(CaseClosed e)
            => _store.Update(e);

        public void Handle(CaseConnectedToSurveyResponse e)
            => _store.Update(e);

        public void Handle(CaseDescribed e)
            => _store.Update(e);

        public void Handle(CaseOpened2 e)
            => _store.Insert(e);

        public void Handle(CaseReopened e)
            => _store.Update(e);

        public void Handle(CaseStatusChanged e)
            => _store.Update(e);

        public void Handle(CaseTitleChanged e)
            => _store.Update(e);

        public void Handle(CaseTypeChanged e)
            => _store.Update(e);

        public void Handle(CaseDeleted e)
            => _store.Delete(e.AggregateIdentifier);

        public void Handle(UserAssigned e)
            => _store.Update(e);

        public void Handle(UserUnassigned e)
            => _store.Update(e);

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType == ObsoleteChangeType.CaseOpened)
            {
                var v2 = CaseOpened2.Upgrade(e);
                Handle(v2);
            }
        }

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear all the existing data in the query store for this projection.
            if (id.HasValue)
                _store.Delete(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Case", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Case", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
