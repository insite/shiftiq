using System;

using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ReconfigureCriterionTab : Command
    {
        public Guid Criterion { get; set; }
        public bool WarningOnNextTabEnabled { get; set; }
        public bool BreakTimerEnabled { get; set; }
        public int TimeLimit { get; set; }
        public FormSectionTimeType TimerType { get; set; }

        public ReconfigureCriterionTab(Guid bank, Guid criterion, bool warningOnNextTab, bool breakTimerEnabled, int timeLimit, FormSectionTimeType timerType)
        {
            AggregateIdentifier = bank;
            Criterion = criterion;
            WarningOnNextTabEnabled = warningOnNextTab;
            BreakTimerEnabled = breakTimerEnabled;
            TimeLimit = timeLimit;
            TimerType = timerType;
        }
    }
}
