using System;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricCriterion
    {
        public Guid Id { get; set; }
        public bool IsRange { get; set; }

        [JsonProperty]
        public ContentContainer Content { get; private set; }

        [JsonIgnore]
        public decimal Points => Ratings.Count == 0 ? 0 : Ratings.Max(x => x.Points);

        [JsonProperty]
        public RubricRatingCollection Ratings { get; private set; }

        public RubricCriterion()
        {
            Content = new ContentContainer();
            Ratings = new RubricRatingCollection(this);
        }

        public RubricRating FindRating(Guid id) => Ratings.FirstOrDefault(x => x.Id == id);

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Ratings.SetContainer(this);
        }

        #endregion
    }
}
