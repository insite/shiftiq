using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GradeCriterionItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid CriterionId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRange { get; set; }
        public string Points { get; set; }

        public GradeRatingItem SelectedRating => Ratings.FirstOrDefault(x => x.SelectedPoints.HasValue);

        [JsonProperty(PropertyName = "ratings")]
        public List<GradeRatingItem> Ratings { get; set; }

        public bool Load(GradeCriterionItem item)
        {
            var isValid = CriterionId == item.CriterionId
                && Ratings.Count == item.Ratings.Count
                && Ratings.Zip(item.Ratings, (a, b) => a.RatingId == b.RatingId).All(x => x);

            if (isValid)
            {
                for (var i = 0; i < Ratings.Count; i++)
                    Ratings[i].SelectedPoints = item.Ratings[i].SelectedPoints;
            }

            return isValid;
        }
    }
}