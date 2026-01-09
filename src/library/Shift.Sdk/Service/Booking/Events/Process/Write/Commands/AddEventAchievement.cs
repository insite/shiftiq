using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class AddEventAchievement : Command
    {
        public Guid Achievement { get; set; }

        public AddEventAchievement(Guid @event, Guid achievement)
        {
            AggregateIdentifier = @event;
            Achievement = achievement;
        }
    }
}
