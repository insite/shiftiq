using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SectionTabConfiguration
    {
        /// <summary>
        /// Determines whether the user is shown a warning before navigating to the next tab.
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool WarningOnNextTabEnabled { get; set; } = true;

        /// <summary>
        /// Determines whether the time spent on this tab (section) is excluded from the attempt's total time.
        /// </summary>
        public bool BreakTimerEnabled { get; set; }

        /// <summary>
        /// The maximum number of minutes during which the user can stay on this tab (section).
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// Determines whether the user can leave the tab before the time specified in <see cref="TimeLimit"/> elapses
        /// (Optional allows early exit; Enforced requires the full time to pass).
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FormSectionTimeType TimerType { get; set; } = FormSectionTimeType.Optional;

        public SectionTabConfiguration Clone()
        {
            return new SectionTabConfiguration
            {
                WarningOnNextTabEnabled = WarningOnNextTabEnabled,
                BreakTimerEnabled = BreakTimerEnabled,
                TimeLimit = TimeLimit,
                TimerType = TimerType
            };
        }
    }
}
