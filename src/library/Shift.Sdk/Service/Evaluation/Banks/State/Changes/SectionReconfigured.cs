using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SectionReconfigured : Change
    {
        public Guid Section { get; set; }
        public bool WarningOnNextTabEnabled { get; set; }
        public bool BreakTimerEnabled { get; set; }
        public int TimeLimit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; set; }

        public SectionReconfigured(Guid section, bool warningOnNextTab, bool breakTimerEnabled, int timeLimit, FormSectionTimeType timerType)
        {
            Section = section;
            WarningOnNextTabEnabled = warningOnNextTab;
            BreakTimerEnabled = breakTimerEnabled;
            TimeLimit = timeLimit;
            TimerType = timerType;
        }
    }
}
