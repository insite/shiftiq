using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class QuestionAdded : Change
    {
        public Guid Set { get; set; }
        public Guid Question { get; set; }
        public Guid Standard { get; set; }
        public Guid? Source { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionItemType Type { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionCalculationMethod Method { get; set; }

        public string Condition { get; set; }

        public int Asset { get; set; }
        public decimal? Points { get; set; }

        public ContentExamQuestion Content { get; set; }

        public QuestionAdded(
            Guid set, Guid question, Guid standard, Guid? source,
            QuestionItemType type, QuestionCalculationMethod method,
            string condition, int asset, decimal? points, ContentExamQuestion content)
        {
            Set = set;
            Question = question;
            Standard = standard;
            Source = source;

            Type = type;
            Method = method;

            Condition = condition;

            Asset = asset;
            Points = points;

            Content = content;
        }
    }
}
