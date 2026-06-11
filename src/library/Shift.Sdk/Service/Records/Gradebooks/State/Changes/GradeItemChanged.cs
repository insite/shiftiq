using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class GradeItemChanged : Change
    {
        public GradeItemChanged(
            Guid item,
            string code,
            string name,
            string shortName,
            bool isReported,
            GradeItemFormat format,
            GradeItemType type,
            GradeItemWeighting weighting,
            Guid? parent
            )
        {
            Item = item;
            Code = code;
            Name = name;
            ShortName = shortName;
            IsReported = isReported;

            Format = format;
            Type = type;
            Weighting = weighting;

            Parent = parent;
        }

        public Guid Item { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsReported { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GradeItemFormat Format { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GradeItemType Type { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GradeItemWeighting Weighting { get; set; }

        public Guid? Parent { get; set; }
    }
}
