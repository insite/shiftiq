using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventVenueChanged : Change
    {
        public Guid? Venue { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Office { get; set; }
        public string Url { get; set; }

        public EventVenueChanged(Guid? id, string name, string location, string office, string url)
        {
            Venue = id;
            Name = name;
            Location = location;
            Office = office;
            Url = url;
        }
    }

    public class EventVenueChanged2 : Change
    {
        public Guid? Office { get; set; }
        public Guid? Location { get; set; }
        public string Room { get; set; }

        public EventVenueChanged2(Guid? office, Guid? location, string room)
        {
            Office = office;
            Location = location;
            Room = room;
        }
    }
}