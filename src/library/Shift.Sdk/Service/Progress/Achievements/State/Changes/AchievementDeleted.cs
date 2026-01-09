
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementDeleted : Change
    {
        public bool Cascade { get; set; }

        public AchievementDeleted(bool cascade = false)
        {
            Cascade = cascade;
        }
    }
}