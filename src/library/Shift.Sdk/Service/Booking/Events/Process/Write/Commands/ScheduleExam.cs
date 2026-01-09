using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ScheduleExam : Command
    {
        public Guid Tenant { get; set; }
        public string ExamType { get; set; }
        public int ExamDuration { get; set; }

        public DateTimeOffset EventStartTime { get; set; }
        
        public string EventFormat { get; set; }
        public int EventNumber { get; set; }
        public string EventTitle { get; set; }
        public string EventStatus { get; set; }
        public string EventClassCode { get; set; }
        public string EventBillingCode { get; set; }
        public string EventSource { get; set; }

        public Guid VenueIdentifier { get; set; }
        public string VenueRoom { get; set; }

        public int? CapacityMaximum { get; set; }
        public int? CapacityMinimum { get; set; }

        public ScheduleExam(Guid id, Guid tenant)
        {
            AggregateIdentifier = id;
            Tenant = tenant;
        }
    }
}
