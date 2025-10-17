using System;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GradeRatingItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid RatingId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Points { get; set; }
        public decimal CurrentPoints { get; set; }
        public decimal PrevPoints { get; set; }

        [JsonProperty(PropertyName = "points")]
        public decimal? SelectedPoints { get; set; }
    }
}