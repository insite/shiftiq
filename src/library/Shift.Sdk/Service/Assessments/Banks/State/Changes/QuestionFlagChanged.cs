using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class QuestionFlagChanged : Change
    {
        public Guid Question { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FlagType Flag { get; set; }

        public QuestionFlagChanged(Guid question, FlagType flag)
        {
            Question = question;
            Flag = flag;
        }
    }
}
