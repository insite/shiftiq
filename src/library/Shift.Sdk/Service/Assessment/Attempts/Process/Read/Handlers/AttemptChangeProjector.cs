using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Banks.Read;
using InSite.Domain.Attempts;

namespace InSite.Application.Attempts.Read
{
    /// <summary>
    /// Implements the projector for changes to an assessment Attempt.
    /// </summary>
    /// <remarks>
    /// Methods in this class should modify only the query/projection tables for assessment attempts. They must NOT have any other side effects.
    /// </remarks>
    public class AttemptChangeProjector
    {
        private readonly IInstructorAttemptStore _attemptStore;
        private readonly IBankSearch _banks;

        public AttemptChangeProjector(
            IChangeQueue publisher,
            IChangeStore changeStore,
            IInstructorAttemptStore attemptStore,
            IBankSearch banks
            )
        {
            _attemptStore = attemptStore;

            _banks = banks;

            publisher.Subscribe<AttemptAnalyzed>(Handle);
            publisher.Subscribe<AttemptSubmitted>(Handle);
            publisher.Subscribe<AttemptGraded>(Handle);
            publisher.Subscribe<AttemptGradedDateChanged>(Handle);
            publisher.Subscribe<AttemptFixed>(Handle);
            publisher.Subscribe<AttemptImported>(Handle);
            publisher.Subscribe<AttemptPinged>(Handle);
            publisher.Subscribe<AttemptResumed>(Handle);
            publisher.Subscribe<AttemptStarted2>(Handle);
            publisher.Subscribe<AttemptStarted3>(Handle);
            publisher.Subscribe<AttemptTagged>(Handle);
            publisher.Subscribe<AttemptVoided>(Handle);
            publisher.Subscribe<AttemptCommentPosted>(Handle);
            publisher.Subscribe<ComposedQuestionAnswered>(Handle);
            publisher.Subscribe<ComposedQuestionAttemptStarted>(Handle);
            publisher.Subscribe<ComposedQuestionScored>(Handle);
            publisher.Subscribe<MatchingQuestionAnswered>(Handle);
            publisher.Subscribe<MultipleChoiceQuestionAnswered>(Handle);
            publisher.Subscribe<MultipleCorrectQuestionAnswered>(Handle);
            publisher.Subscribe<QuestionVoided>(Handle);
            publisher.Subscribe<QuestionRegraded>(Handle);
            publisher.Subscribe<ScoreCalculated>(Handle);
            publisher.Subscribe<BooleanTableQuestionAnswered>(Handle);
            publisher.Subscribe<TrueOrFalseQuestionAnswered>(Handle);
            publisher.Subscribe<HotspotQuestionAnswered>(Handle);
            publisher.Subscribe<OrderingQuestionAnswered>(Handle);
            publisher.Subscribe<AttemptSectionSwitched>(Handle);
            publisher.Subscribe<AttemptQuestionSwitched>(Handle);
            publisher.Subscribe<AttemptGradingAssessorAssigned>(Handle);
            publisher.Subscribe<AttemptRubricPointsUpdated>(Handle);
            publisher.Subscribe<AttemptRubricChanged>(Handle);
            publisher.Subscribe<AttemptQuestionRubricChanged>(Handle);
            publisher.Subscribe<AttemptQuestionRubricInited>(Handle);

            changeStore.RegisterObsoleteChangeTypes(new[]
            {
                ObsoleteChangeType.AttemptStarted,
                ObsoleteChangeType.AttemptFlushed
            });
        }

        /// <summary>
        /// Moves the buffered attempt to the query table for instructors and administrators.
        /// </summary>
        public void Handle(AttemptAnalyzed e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptSubmitted e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptGraded e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptGradedDateChanged e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptFixed e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptImported e)
            => _attemptStore.InsertAttempt(e, _banks.GetFormData(e.Form));

        public void Handle(AttemptPinged e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptResumed e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptStarted2 change)
            => _attemptStore.InsertAttempt(change, _banks.GetFormData(change.FormIdentifier));

        public void Handle(AttemptStarted3 change)
            => _attemptStore.InsertAttempt(change);

        public void Handle(AttemptTagged e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptVoided e) =>
            _attemptStore.DeleteAttempt(e);

        public void Handle(AttemptCommentPosted e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(ComposedQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(ComposedQuestionAttemptStarted e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(ComposedQuestionScored e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(MatchingQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(MultipleChoiceQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(MultipleCorrectQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(QuestionVoided e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(QuestionRegraded e)
            => _attemptStore.UpdateAttempt(e, _banks.GetFormData(e.Form));

        public void Handle(ScoreCalculated e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(TrueOrFalseQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(BooleanTableQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(HotspotQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(OrderingQuestionAnswered e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptSectionSwitched e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptQuestionSwitched e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptGradingAssessorAssigned e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptRubricPointsUpdated e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptRubricChanged e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptQuestionRubricChanged e)
            => _attemptStore.UpdateAttempt(e);

        public void Handle(AttemptQuestionRubricInited e) { }

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ObsoleteChangeType.AttemptStarted)
                return;

            var v2 = AttemptStarted2.Upgrade(e);

            Handle(v2);
        }

        public void Replay(IChangeStore store, IEnumerable<Guid> aggregateIdentifiers, Action<string, int, int, Guid> progress)
        {
            if (aggregateIdentifiers == null)
                throw new ArgumentNullException(nameof(aggregateIdentifiers));

            // Clear all the existing data in the query store for this projection.

            foreach (var id in aggregateIdentifiers)
                _attemptStore.DeleteAttempt(id);

            // Get all the bank aggregates.
            var ids = store.GetAggregates("Attempt");

            var aggregates = new Dictionary<Guid, AttemptAggregate>();

            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                if (aggregateIdentifiers != null && !aggregateIdentifiers.Contains(id))
                    continue;

                var aggregate = AggregateFactory<AttemptAggregate>.CreateAggregate();
                aggregate.AggregateIdentifier = id;
                aggregate.State = aggregate.CreateState();

                aggregates.Add(id, aggregate);
            }

            if (aggregates.Count == 0)
                return;

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Attempt", aggregates.Keys);
            var changeNumber = 0;

            // Handle each of the events in the order they occurred.
            foreach (var change in changes)
            {
                var aggregate = aggregates[change.AggregateIdentifier];

                progress($"    Version {change.AggregateVersion:000000}", ++changeNumber, changes.Count, change.AggregateIdentifier);

                aggregate.State.Apply(change);
                change.AggregateState = aggregate.State;

                var handler = GetType().GetMethod("Handle", new Type[] { change.GetType() });
                handler.Invoke(this, new[] { change });
            }
        }
    }
}