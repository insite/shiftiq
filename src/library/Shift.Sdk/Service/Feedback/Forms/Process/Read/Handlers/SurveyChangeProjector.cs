using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Read
{
    /// <summary>
    /// Implements the process manager for Survey events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class SurveyChangeProjector
    {
        private readonly ISurveyStore _store;

        public SurveyChangeProjector(IChangeQueue publisher, ISurveyStore store)
        {
            _store = store;

            publisher.Subscribe<SurveyBranchAdded>(Handle);
            publisher.Subscribe<SurveyBranchDeleted>(Handle);
            publisher.Subscribe<SurveyCommentDeleted>(Handle);
            publisher.Subscribe<SurveyCommentModified>(Handle);
            publisher.Subscribe<SurveyCommentPosted>(Handle);
            publisher.Subscribe<SurveyConditionAdded>(Handle);
            publisher.Subscribe<SurveyConditionDeleted>(Handle);
            publisher.Subscribe<SurveyDisplaySummaryChartChanged>(Handle);
            publisher.Subscribe<SurveyFormAssetChanged>(Handle);
            publisher.Subscribe<SurveyFormContentChanged>(Handle);
            publisher.Subscribe<SurveyFormCreated>(Handle);
            publisher.Subscribe<SurveyFormLanguagesChanged>(Handle);
            publisher.Subscribe<SurveyFormLocked>(Handle);
            publisher.Subscribe<SurveyFormMessageAdded>(Handle);
            publisher.Subscribe<SurveyFormMessagesChanged>(Handle);
            publisher.Subscribe<SurveyFormRenamed>(Handle);
            publisher.Subscribe<SurveyHookChanged>(Handle);
            publisher.Subscribe<SurveyFormScheduleChanged>(Handle);
            publisher.Subscribe<SurveyFormSettingsChanged>(Handle);
            publisher.Subscribe<SurveyFormStatusChanged>(Handle);
            publisher.Subscribe<SurveyFormDeleted>(Handle);
            publisher.Subscribe<SurveyFormUnlocked>(Handle);
            publisher.Subscribe<SurveyOptionItemAdded>(Handle);
            publisher.Subscribe<SurveyOptionItemContentChanged>(Handle);
            publisher.Subscribe<SurveyOptionItemDeleted>(Handle);
            publisher.Subscribe<SurveyOptionItemSettingsChanged>(Handle);
            publisher.Subscribe<SurveyOptionItemsReordered>(Handle);
            publisher.Subscribe<SurveyOptionListAdded>(Handle);
            publisher.Subscribe<SurveyOptionListContentChanged>(Handle);
            publisher.Subscribe<SurveyOptionListDeleted>(Handle);
            publisher.Subscribe<SurveyOptionListsReordered>(Handle);
            publisher.Subscribe<SurveyQuestionAdded>(Handle);
            publisher.Subscribe<SurveyQuestionAttributed>(Handle);
            publisher.Subscribe<SurveyQuestionContentChanged>(Handle);
            publisher.Subscribe<SurveyQuestionRecoded>(Handle);
            publisher.Subscribe<SurveyQuestionDeleted>(Handle);
            publisher.Subscribe<SurveyQuestionSettingsChanged>(Handle);
            publisher.Subscribe<SurveyQuestionsReordered>(Handle);
            publisher.Subscribe<SurveyRespondentsAdded>(change => { });
            publisher.Subscribe<SurveyRespondentsDeleted>(change => { });
            publisher.Subscribe<SurveyScaleChanged>(Handle);
            publisher.Subscribe<SurveyWorkflowConfigured>(Handle);
        }

        public void Handle(SurveyBranchAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyBranchDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyCommentDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyCommentModified e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyCommentPosted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyConditionAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyConditionDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyDisplaySummaryChartChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormAssetChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormContentChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormCreated e)
            => _store.InsertSurvey(e);

        public void Handle(SurveyFormLanguagesChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormLocked e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormMessageAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormMessagesChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormRenamed e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyHookChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormScheduleChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormSettingsChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormStatusChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormUnlocked e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyFormDeleted e)
            => _store.DeleteSurvey(e);

        public void Handle(SurveyOptionItemAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionItemContentChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionItemDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionItemSettingsChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionItemsReordered e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionListAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionListContentChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionListDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyOptionListsReordered e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionAdded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionAttributed e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionContentChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionRecoded e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionDeleted e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionSettingsChanged e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyQuestionsReordered e)
            => _store.UpdateSurvey(e);

        public void Handle(SurveyScaleChanged e) { }

        public void Handle(SurveyWorkflowConfigured e)
            => _store.UpdateSurvey(e);

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear all the existing data in the query store for this projection.
            if (id.HasValue)
                _store.DeleteAll(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Survey", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Survey", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}