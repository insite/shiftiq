using System;

namespace InSite.Domain.Registrations
{
    public class AttendeeListReportDataItem
    {
        public string UserFullName { get; set; }
        public string Email { get; set; }
        public string EmployerName { get; set; }
        public string Phone { get; set; }
        public string PersonCode { get; set; }
        public DateTime? CandidateBirthdate { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
    }
}
