using System;

using Newtonsoft.Json;

using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class CriterionTabReconfigured : Change
    {
        public Guid Criterion { get; set; }
        public bool WarningOnNextTabEnabled { get; set; }
        public bool BreakTimerEnabled { get; set; }
        public int TimeLimit { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; set; }

        public CriterionTabReconfigured(Guid criterion, bool warningOnNextTab, bool breakTimerEnabled, int timeLimit, FormSectionTimeType timerType)
        {
            Criterion = criterion;
            WarningOnNextTabEnabled = warningOnNextTab;
            BreakTimerEnabled = breakTimerEnabled;
            TimeLimit = timeLimit;
            TimerType = timerType;
        }
    }
}
