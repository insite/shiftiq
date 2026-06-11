using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ReconfigureSection : Command
    {
        public Guid Section { get; set; }
        public bool WarningOnNextTabEnabled { get; set; }
        public bool BreakTimerEnabled { get; set; }
        public int TimeLimit { get; set; }
        public FormSectionTimeType TimerType { get; set; }

        public ReconfigureSection(Guid bank, Guid section, bool warningOnNextTab, bool breakTimerEnabled, int timeLimit, FormSectionTimeType timerType)
        {
            AggregateIdentifier = bank;
            Section = section;
            WarningOnNextTabEnabled = warningOnNextTab;
            BreakTimerEnabled = breakTimerEnabled;
            TimeLimit = timeLimit;
            TimerType = timerType;
        }
    }
}
