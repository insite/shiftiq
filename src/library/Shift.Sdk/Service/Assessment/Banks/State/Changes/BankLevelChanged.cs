using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class BankLevelChanged : Change
    {
        public Level Level { get; set; }

        public BankLevelChanged(Level level)
        {
            Level = level;
        }
    }
}
