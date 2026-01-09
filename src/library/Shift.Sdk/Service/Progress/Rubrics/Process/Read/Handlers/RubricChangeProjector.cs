using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public class RubricChangeProjector
    {
        private readonly IRubricStore _rubricStore;

        public RubricChangeProjector(IChangeQueue publisher, IRubricStore rubricStore)
        {
            _rubricStore = rubricStore;

            publisher.Subscribe<RubricCreated>(Handle);
            publisher.Subscribe<RubricDeleted>(Handle);
            publisher.Subscribe<RubricRenamed>(Handle);
            publisher.Subscribe<RubricDescribed>(Handle);
            publisher.Subscribe<RubricTimestampModified>(Handle);
            publisher.Subscribe<RubricTranslated>(Handle);

            publisher.Subscribe<RubricCriterionAdded>(Handle);
            publisher.Subscribe<RubricCriterionRemoved>(Handle);
            publisher.Subscribe<RubricCriterionRenamed>(Handle);
            publisher.Subscribe<RubricCriterionDescribed>(Handle);
            publisher.Subscribe<RubricCriterionIsRangeModified>(Handle);
            publisher.Subscribe<RubricCriterionTranslated>(Handle);

            publisher.Subscribe<RubricCriterionRatingAdded>(Handle);
            publisher.Subscribe<RubricCriterionRatingRemoved>(Handle);
            publisher.Subscribe<RubricCriterionRatingRenamed>(Handle);
            publisher.Subscribe<RubricCriterionRatingDescribed>(Handle);
            publisher.Subscribe<RubricCriterionRatingPointsModified>(Handle);
            publisher.Subscribe<RubricCriterionRatingTranslated>(Handle);
        }

        public void Handle(RubricCreated e) => _rubricStore.Insert(e);

        public void Handle(RubricDeleted e) => _rubricStore.Delete(e);

        public void Handle(RubricRenamed e) => _rubricStore.Update(e);

        public void Handle(RubricDescribed e) => _rubricStore.Update(e);

        public void Handle(RubricTimestampModified e) => _rubricStore.Update(e);

        public void Handle(RubricTranslated e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionAdded e) => _rubricStore.Insert(e);

        public void Handle(RubricCriterionRemoved e) => _rubricStore.Delete(e);

        public void Handle(RubricCriterionRenamed e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionDescribed e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionIsRangeModified e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionTranslated e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionRatingAdded e) => _rubricStore.Insert(e);

        public void Handle(RubricCriterionRatingRemoved e) => _rubricStore.Delete(e);

        public void Handle(RubricCriterionRatingRenamed e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionRatingDescribed e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionRatingPointsModified e) => _rubricStore.Update(e);

        public void Handle(RubricCriterionRatingTranslated e) => _rubricStore.Update(e);

        public void Replay(IChangeStore store, IEnumerable<Guid> aggregateIds, Action<string, int, int, Guid> progress)
        {
            // Clear all the existing data in the query store for this projection.
            if (aggregateIds != null)
            {
                foreach (var id in aggregateIds)
                    _rubricStore.Delete(id);
            }
            else
                _rubricStore.DeleteAll();

            // Get all the rubric aggregates.
            var ids = store.GetAggregates("Rubric");

            var aggregates = new Dictionary<Guid, RubricAggregate>();

            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                if (aggregateIds != null && !aggregateIds.Contains(id))
                    continue;

                var aggregate = AggregateFactory<RubricAggregate>.CreateAggregate();
                aggregate.AggregateIdentifier = id;
                aggregate.State = aggregate.CreateState();

                aggregates.Add(id, aggregate);
            }

            if (aggregates.Count == 0)
                return;

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Rubric", aggregates.Keys);
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
