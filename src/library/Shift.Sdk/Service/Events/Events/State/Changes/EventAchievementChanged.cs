using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventAchievementChanged : Change
    {
        public Guid? Achievement { get; set; }

        public EventAchievementChanged(Guid? achievement)
        {
            Achievement = achievement;
        }
    }
}
