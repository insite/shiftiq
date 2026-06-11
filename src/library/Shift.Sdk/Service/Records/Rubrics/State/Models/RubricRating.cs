using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricRating
    {
        public Guid Id { get; set; }
        public decimal Points { get; set; }

        [JsonProperty]
        public ContentContainer Content { get; private set; } = new ContentContainer();

        [JsonIgnore]
        public RubricCriterion Criterion => _criterion;

        [NonSerialized]
        private RubricCriterion _criterion;

        public void SetContainer(RubricCriterion criterion)
        {
            if (_criterion != null)
                throw ApplicationError.Create("Criterion is already assigned to this rating");

            _criterion = criterion;
        }

        public void RemoveContainer() => _criterion = null;
    }
}
