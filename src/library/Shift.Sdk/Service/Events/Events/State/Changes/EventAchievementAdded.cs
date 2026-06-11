using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventAchievementAdded : Change
    {
        public Guid Achievement { get; set; }

        public EventAchievementAdded(Guid achievement)
        {
            Achievement = achievement;
        }
    }
}
