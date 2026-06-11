using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventAchievement : Command
    {
        public Guid? Achievement { get; set; }

        public ChangeEventAchievement(Guid id, Guid? achievement)
        {
            AggregateIdentifier = id;
            Achievement = achievement;
        }
    }
}
