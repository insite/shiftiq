using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ScheduleMeeting : Command
    {
        public Guid Tenant { get; set; }
        public DateTimeOffset EventStartTime { get; set; }
        public DateTimeOffset EventEndTime { get; set; }

        public int EventNumber { get; set; }
        public string EventTitle { get; set; }
        public string EventStatus { get; set; }
        public string EventDescription { get; set; }

        public Guid VenueIdentifier { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public string VenueOffice { get; set; }
        public string VenueUrl { get; set; }

        public ScheduleMeeting(Guid id, Guid tenant)
        {
            AggregateIdentifier = id;
            Tenant = tenant;
        }
    }
}
