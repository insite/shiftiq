using System;

using Newtonsoft.Json;

using Shift.Common;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class PageNode
    {
        [JsonProperty]
        public Guid Identifier { get; set; }

        [JsonProperty]
        public Guid? Parent { get; set; }

        [JsonProperty]
        public string Icon { get; set; }

        [JsonProperty]
        public int Sequence { get; set; }

        [JsonProperty]
        public string Slug { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public bool IsHidden { get; set; }

        [JsonProperty]
        public DateTimeOffset Modified { get; set; }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public DateTimeOffset? Authored { get; set; }

        public string Posted => ShiftHumanizer.Humanize(Authored ?? DateTimeOffset.UtcNow);

        [JsonProperty]
        public string Control { get; set; }

        [JsonProperty]
        public string NavigateUrl { get; set; }

        [JsonProperty]
        public ContentContainer Content { get; set; }
    }
}
