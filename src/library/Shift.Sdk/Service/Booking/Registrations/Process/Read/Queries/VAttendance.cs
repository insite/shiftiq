using System;

namespace InSite.Application.Registrations.Read
{
    public class VAttendance
    {
        public Guid AssessmentFormIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AssessmentFormCode { get; set; }
        public string AssessmentFormName { get; set; }
        public string AssessmentFormTitle { get; set; }
        public string AttendanceStatus { get; set; }
        public string EventFormat { get; set; }
        public string LastChangeType { get; set; }
        public string LearnerCode { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }

        public int EventNumber { get; set; }

        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
    }
}
