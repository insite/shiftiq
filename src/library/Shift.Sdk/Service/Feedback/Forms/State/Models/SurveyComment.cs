using System;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyComment
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public FlagType? Flag { get; set; }
        public DateTimeOffset? Resolved { get; set; }
    }
}
