using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Read
{
    /// <summary>
    /// Implements the projector for Bank events.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class BankChangeProjector
    {
        private static readonly HashSet<string> _obsoleteChanges = new HashSet<string>
        {
            "QuestionTriggerAdded",
            "QuestionTriggerChanged",
            "QuestionTriggerDeleted",
            "BankGradebookChanged",
            "QuestionGradeItemChanged"
        };

        #region Methods (construction)

        private readonly IBankStore _store;

        public BankChangeProjector(IChangeQueue queue, IChangeStore changeStore, IBankStore store)
        {
            queue.Subscribe<BankMemorized>(Handle);
            queue.Subscribe<AssessmentHookChanged>(Handle);
            queue.Subscribe<AttachmentAdded>(Handle);
            queue.Subscribe<AttachmentAddedToQuestion>(Handle);
            queue.Subscribe<AttachmentChanged>(Handle);
            queue.Subscribe<AttachmentImageChanged>(Handle);
            queue.Subscribe<BankAttachmentDeleted>(Handle);
            queue.Subscribe<AttachmentDeletedFromQuestion>(Handle);
            queue.Subscribe<AttachmentUpgraded>(Handle);
            queue.Subscribe<BankAnalyzed>(Handle);
            queue.Subscribe<BankDeleted>(Handle);
            queue.Subscribe<BankContentChanged>(Handle);
            queue.Subscribe<BankOpened>(Handle);
            queue.Subscribe<BankLevelChanged>(Handle);
            queue.Subscribe<FormMessageConnected>(Handle);
            queue.Subscribe<BankLocked>(Handle);
            queue.Subscribe<BankRenamed>(Handle);
            queue.Subscribe<BankStandardChanged>(Handle);
            queue.Subscribe<BankTypeChanged>(Handle);
            queue.Subscribe<BankUnlocked>(Handle);
            queue.Subscribe<BankEditionChanged>(Handle);
            queue.Subscribe<BankStatusChanged>(Handle);
            queue.Subscribe<CommentAuthorRoleChanged>(Handle);
            queue.Subscribe<CommentDuplicated>(Handle);
            queue.Subscribe<CommentMoved>(Handle);
            queue.Subscribe<BankCommentPosted>(Handle);
            queue.Subscribe<CommentRejected>(Handle);
            queue.Subscribe<CommentRetracted>(Handle);
            queue.Subscribe<BankCommentModified>(Handle);
            queue.Subscribe<CommentVisibilityChanged>(Handle);
            queue.Subscribe<FieldAdded>(Handle);
            queue.Subscribe<FieldDeleted>(Handle);
            queue.Subscribe<FieldsDeleted>(Handle);
            queue.Subscribe<FieldsReordered>(Handle);
            queue.Subscribe<FieldsSwapped>(Handle);
            queue.Subscribe<FormAdded>(Handle);
            queue.Subscribe<FormAddendumChanged>(Handle);
            queue.Subscribe<FormAnalyzed>(Handle);
            queue.Subscribe<FormArchived>(Handle);
            queue.Subscribe<FormAssetChanged>(Handle);
            queue.Subscribe<FormClassificationChanged>(Handle);
            queue.Subscribe<FormCodeChanged>(Handle);
            queue.Subscribe<FormContentChanged>(Handle);
            queue.Subscribe<FormGradebookChanged>(Handle);
            queue.Subscribe<FormInvigilationChanged>(Handle);
            queue.Subscribe<FormLanguagesModified>(Handle);
            queue.Subscribe<FormNameChanged>(Handle);
            queue.Subscribe<FormPublished>(Handle);
            queue.Subscribe<FormDeleted>(Handle);
            queue.Subscribe<FormUnarchived>(Handle);
            queue.Subscribe<FormUnpublished>(Handle);
            queue.Subscribe<FormUpgraded>(Handle);
            queue.Subscribe<FormVersionChanged>(Handle);
            queue.Subscribe<OptionAdded>(Handle);
            queue.Subscribe<OptionChanged>(Handle);
            queue.Subscribe<OptionDeleted>(Handle);
            queue.Subscribe<OptionsReordered>(Handle);
            queue.Subscribe<QuestionAdded>(Handle);
            queue.Subscribe<QuestionClassificationChanged>(Handle);
            queue.Subscribe<QuestionComposedVoiceChanged>(Handle);
            queue.Subscribe<QuestionConditionChanged>(Handle);
            queue.Subscribe<QuestionContentChanged>(Handle);
            queue.Subscribe<QuestionDuplicated2>(Handle);
            queue.Subscribe<QuestionFlagChanged>(Handle);
            queue.Subscribe<QuestionGradeItemChanged2>(Handle);
            queue.Subscribe<QuestionLikertRowGradeItemChanged>(Handle);
            queue.Subscribe<QuestionHotspotImageChanged>(Handle);
            queue.Subscribe<QuestionHotspotOptionAdded>(Handle);
            queue.Subscribe<QuestionHotspotOptionChanged>(Handle);
            queue.Subscribe<QuestionHotspotOptionDeleted>(Handle);
            queue.Subscribe<QuestionHotspotOptionsReordered>(Handle);
            queue.Subscribe<QuestionHotspotPinLimitChanged>(Handle);
            queue.Subscribe<QuestionHotspotShowShapesChanged>(Handle);
            queue.Subscribe<QuestionLayoutChanged>(Handle);
            queue.Subscribe<QuestionLikertColumnAdded>(Handle);
            queue.Subscribe<QuestionLikertColumnChanged>(Handle);
            queue.Subscribe<QuestionLikertColumnDeleted>(Handle);
            queue.Subscribe<QuestionLikertOptionsChanged>(Handle);
            queue.Subscribe<QuestionLikertReordered>(Handle);
            queue.Subscribe<QuestionLikertRowAdded>(Handle);
            queue.Subscribe<QuestionLikertRowChanged>(Handle);
            queue.Subscribe<QuestionLikertRowDeleted>(Handle);
            queue.Subscribe<QuestionMatchesChanged>(Handle);
            queue.Subscribe<QuestionMoved>(Handle);
            queue.Subscribe<QuestionMovedIn>(Handle);
            queue.Subscribe<QuestionMovedOut>(Handle);
            queue.Subscribe<QuestionOrderingOptionAdded>(Handle);
            queue.Subscribe<QuestionOrderingSolutionAdded>(Handle);
            queue.Subscribe<QuestionOrderingLabelChanged>(Handle);
            queue.Subscribe<QuestionOrderingOptionChanged>(Handle);
            queue.Subscribe<QuestionOrderingSolutionChanged>(Handle);
            queue.Subscribe<QuestionOrderingOptionDeleted>(Handle);
            queue.Subscribe<QuestionOrderingSolutionDeleted>(Handle);
            queue.Subscribe<QuestionOrderingOptionsReordered>(Handle);
            queue.Subscribe<QuestionOrderingSolutionsReordered>(Handle);
            queue.Subscribe<QuestionOrderingSolutionOptionsReordered>(Handle);
            queue.Subscribe<QuestionPublicationStatusChanged>(Handle);
            queue.Subscribe<QuestionRandomizationChanged>(Handle);
            queue.Subscribe<QuestionRubricConnected>(Handle);
            queue.Subscribe<QuestionRubricDisconnected>(Handle);
            queue.Subscribe<QuestionDeleted>(Handle);
            queue.Subscribe<QuestionScoringChanged>(Handle);
            queue.Subscribe<QuestionSetChanged>(Handle);
            queue.Subscribe<QuestionStandardChanged>(Handle);
            queue.Subscribe<QuestionUpgraded>(Handle);
            queue.Subscribe<QuestionsReordered>(Handle);
            queue.Subscribe<SectionAdded>(Handle);
            queue.Subscribe<SectionDeleted>(Handle);
            queue.Subscribe<SectionReconfigured>(Handle);
            queue.Subscribe<SectionsReordered>(Handle);
            queue.Subscribe<ThirdPartyAssessmentEnabled>(Handle);
            queue.Subscribe<ThirdPartyAssessmentDisabled>(Handle);
            queue.Subscribe<SetAdded>(Handle);
            queue.Subscribe<SectionContentChanged>(Handle);
            queue.Subscribe<SetImported>(Handle);
            queue.Subscribe<SetRandomizationChanged>(Handle);
            queue.Subscribe<SetDeleted>(Handle);
            queue.Subscribe<SetRenamed>(Handle);
            queue.Subscribe<SetsMerged>(Handle);
            queue.Subscribe<SetsReordered>(Handle);
            queue.Subscribe<SetStandardChanged>(Handle);
            queue.Subscribe<CriterionAdded>(Handle);
            queue.Subscribe<CriterionFilterChanged>(Handle);
            queue.Subscribe<CriterionFilterDeleted>(Handle);
            queue.Subscribe<CriterionDeleted>(Handle);
            queue.Subscribe<SpecificationAdded>(Handle);
            queue.Subscribe<SpecificationCalculationChanged>(Handle);
            queue.Subscribe<SpecificationContentChanged>(Handle);
            queue.Subscribe<SpecificationTabTimeLimitChanged>(Handle);
            queue.Subscribe<SpecificationReconfigured>(Handle);
            queue.Subscribe<SpecificationDeleted>(Handle);
            queue.Subscribe<SpecificationRenamed>(Handle);
            queue.Subscribe<SpecificationRetyped>(Handle);
            queue.Subscribe<SectionsAsTabsDisabled>(Handle);
            queue.Subscribe<SectionsAsTabsEnabled>(Handle);
            queue.Subscribe<TabNavigationEnabled>(Handle);
            queue.Subscribe<TabNavigationDisabled>(Handle);
            queue.Subscribe<SingleQuestionPerTabEnabled>(Handle);
            queue.Subscribe<SingleQuestionPerTabDisabled>(Handle);
            queue.Subscribe<AssessmentQuestionOrderVerified>(Handle);

            changeStore.RegisterObsoleteChangeTypes(_obsoleteChanges);

            _store = store;
        }

        #endregion

        public void Handle(BankMemorized e)
        {
            
        }

        public void Handle(AssessmentHookChanged e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentImageChanged e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentAdded e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentAddedToQuestion e)
        {
            _store.Update(e);
        }

        public void Handle(BankAttachmentDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentDeletedFromQuestion e)
        {
            _store.Update(e);
        }

        public void Handle(AttachmentUpgraded e)
        {
            _store.Update(e);
        }

        public void Handle(BankAnalyzed e)
        {
            _store.Update(e);
        }

        public void Handle(BankDeleted e)
        {
            _store.Delete(e.AggregateIdentifier);
        }

        public void Handle(BankContentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormMessageConnected e)
        {
            _store.Update(e);
        }

        public void Handle(BankOpened e)
        {
            _store.Insert(e);
        }

        public void Handle(BankLevelChanged e)
        {
            _store.Update(e);
        }

        public void Handle(BankLocked e) => _store.Update(e);

        public void Handle(BankRenamed e)
        {
            _store.Update(e);
        }

        public void Handle(BankStandardChanged e)
        {
            _store.Update(e);
        }

        public void Handle(BankTypeChanged e) => _store.Update(e);

        public void Handle(BankUnlocked e) => _store.Update(e);

        public void Handle(BankEditionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(BankStatusChanged e)
        {
            _store.Update(e);
        }

        public void Handle(CommentAuthorRoleChanged e)
        {
            _store.Update(e);
        }

        public void Handle(CommentDuplicated e)
        {
            _store.Update(e);
        }

        public void Handle(CommentMoved e)
        {
            _store.Update(e);
        }

        public void Handle(BankCommentPosted e)
        {
            _store.Update(e);
        }

        public void Handle(CommentRejected e)
        {
            _store.Update(e);
        }

        public void Handle(CommentRetracted e)
        {
            _store.Update(e);
        }

        public void Handle(BankCommentModified e)
        {
            _store.Update(e);
        }

        public void Handle(CommentVisibilityChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FieldsSwapped e)
        {
            _store.Update(e);
        }

        public void Handle(FieldAdded e)
        {
            _store.Update(e);
        }

        public void Handle(FieldDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(FieldsDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(FieldsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(FormAdded e)
        {
            _store.Update(e);
        }

        public void Handle(FormAddendumChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormAnalyzed e)
        {
            _store.Update(e);
        }

        public void Handle(FormArchived e)
        {
            _store.Update(e);
        }

        public void Handle(FormAssetChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormClassificationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormCodeChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormContentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormGradebookChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormInvigilationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormLanguagesModified e)
        {
            _store.Update(e);
        }

        public void Handle(FormNameChanged e)
        {
            _store.Update(e);
        }

        public void Handle(FormPublished e)
        {
            _store.Update(e);
        }

        public void Handle(FormUnarchived e)
            => _store.Update(e);

        public void Handle(FormUnpublished e)
            => _store.Update(e);

        public void Handle(FormUpgraded e)
        {
            _store.Update(e);
        }

        public void Handle(FormDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(FormVersionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(OptionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(OptionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(OptionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(OptionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionClassificationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionComposedVoiceChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionConditionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionContentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionDuplicated2 e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionFlagChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionGradeItemChanged2 e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertRowGradeItemChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotImageChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotOptionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotOptionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotOptionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotOptionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotPinLimitChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionHotspotShowShapesChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLayoutChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertColumnAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertColumnChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertColumnDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertOptionsChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertRowAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertRowChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionLikertRowDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionMatchesChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionMoved e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionMovedIn e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionMovedOut e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingOptionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingSolutionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingLabelChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingOptionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingSolutionChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingOptionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingSolutionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingOptionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingSolutionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionOrderingSolutionOptionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionPublicationStatusChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionRandomizationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionRubricConnected e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionRubricDisconnected e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionScoringChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionSetChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionStandardChanged e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionUpgraded e)
        {
            _store.Update(e);
        }

        public void Handle(QuestionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(SectionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(SectionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(SectionReconfigured e)
        {
            _store.Update(e);
        }

        public void Handle(SectionsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(ThirdPartyAssessmentEnabled e)
            => _store.Update(e);

        public void Handle(ThirdPartyAssessmentDisabled e)
            => _store.Update(e);

        public void Handle(SetAdded e)
        {
            _store.Update(e);
        }

        public void Handle(SetImported e)
        {
            _store.Update(e);
        }

        public void Handle(SetRandomizationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(SectionContentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(SetDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(SetRenamed e)
        {
            _store.Update(e);
        }

        public void Handle(SetsMerged e)
        {
            _store.Update(e);
        }

        public void Handle(SetsReordered e)
        {
            _store.Update(e);
        }

        public void Handle(SetStandardChanged e)
        {
            _store.Update(e);
        }

        public void Handle(CriterionAdded e)
        {
            _store.Update(e);
        }

        public void Handle(CriterionFilterChanged e)
        {
            _store.Update(e);
        }

        public void Handle(CriterionFilterDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(CriterionDeleted e)
        {
            _store.Update(e);
        }

        public void Handle(SpecificationAdded e)
        {
            _store.Update(e);
        }

        public void Handle(SpecificationDeleted e) => _store.Update(e);

        public void Handle(SpecificationRenamed e) => _store.Update(e);

        public void Handle(SpecificationRetyped e) => _store.Update(e);

        public void Handle(SpecificationCalculationChanged e)
        {
            _store.Update(e);
        }

        public void Handle(SpecificationContentChanged e)
        {
            _store.Update(e);
        }

        public void Handle(SpecificationTabTimeLimitChanged e)
        {
            _store.Update(e);
        }

        public void Handle(SpecificationReconfigured e)
        {
            _store.Update(e);
        }

        public void Handle(SectionsAsTabsDisabled e)
        {
            _store.Update(e);
        }

        public void Handle(SectionsAsTabsEnabled e)
        {
            _store.Update(e);
        }

        public void Handle(TabNavigationEnabled e)
        {
            _store.Update(e);
        }

        public void Handle(TabNavigationDisabled e)
        {
            _store.Update(e);
        }

        public void Handle(SingleQuestionPerTabEnabled e)
        {
            _store.Update(e);
        }

        public void Handle(SingleQuestionPerTabDisabled e)
        {
            _store.Update(e);
        }

        public void Handle(AssessmentQuestionOrderVerified verified)
        {

        }

        public void Handle(SerializedChange _)
        {
            // Obsolete changes go here
        }

        public void Replay(IChangeStore store, IEnumerable<Guid> aggregateIdentifiers, Action<string, int, int, Guid> progress)
        {
            // Clear all the existing data in the query store for this projection.
            if (aggregateIdentifiers != null)
            {
                foreach (var id in aggregateIdentifiers)
                    _store.Delete(id);
            }
            else
                _store.DeleteAll();

            // Get all the bank aggregates.
            var ids = store.GetAggregates("Bank");

            var aggregates = new Dictionary<Guid, BankAggregate>();

            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                if (aggregateIdentifiers != null && !aggregateIdentifiers.Contains(id))
                    continue;

                var aggregate = AggregateFactory<BankAggregate>.CreateAggregate();
                aggregate.AggregateIdentifier = id;
                aggregate.State = aggregate.CreateState();

                aggregates.Add(id, aggregate);
            }

            if (aggregates.Count == 0)
                return;

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Bank", aggregates.Keys);
            var changeNumber = 0;

            // Handle each of the events in the order they occurred.
            foreach (var change in changes)
            {
                var aggregate = aggregates[change.AggregateIdentifier];

                progress($"Version {change.AggregateVersion:000000}", ++changeNumber, changes.Count, change.AggregateIdentifier);

                aggregate.State.Apply(change);
                change.AggregateState = aggregate.State;

                var handler = GetType().GetMethod("Handle", new Type[] { change.GetType() });
                handler.Invoke(this, new[] { change });
            }
        }
    }
}