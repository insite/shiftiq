using System;

namespace InSite.Persistence
{
    public class VExamEventSchedule
    {
        public Guid EventIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AccommodationSummary { get; set; }
        public string EventClassCode { get; set; }
        public string EventFormat { get; set; }
        public string EventSchedulingStatus { get; set; }
        public string Eligibility { get; set; }
        public string FormBankTypes { get; set; }
        public string FormBankLevels { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalProvince { get; set; }
        public string Trades { get; set; }
        public string VenueName { get; set; }
        public string VenueOffice { get; set; }
        public string VenueRoom { get; set; }

        public int? AccommodationCount { get; set; }
        public int EventNumber { get; set; }
        public string EventBillingType { get; set; }
        public int? CandidateCount { get; set; }
        public int? CapacityMaximum { get; set; }
        public int? InvigilatorCount { get; set; }
        public int? InvigilatorMinimum { get; set; }

        public DateTimeOffset EventScheduledStart { get; set; }
    }
}
