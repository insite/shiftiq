using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeEventVenue : Command
    {
        public Guid? Office { get; set; }
        public Guid? Location { get; set; }
        public string Room { get; set; }

        public ChangeEventVenue(Guid aggregate, Guid? office, Guid? location, string room)
        {
            AggregateIdentifier = aggregate;
            Office = office;
            Location = location;
            Room = room;
        }
    }
}
