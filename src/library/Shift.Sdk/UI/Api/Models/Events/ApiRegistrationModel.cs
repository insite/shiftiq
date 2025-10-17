using System;

namespace Shift.Sdk.UI
{
    public class ApiRegistrationModel
    {
        public Guid RegistrationIdentifier { get; set; }
        public string RegistrationAttendanceStatus { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }

        public Guid EventIdentifier { get; set; }
        public int EventNumber { get; set; }
        public string EventFormat { get; set; }
        public DateTimeOffset EventStart { get; set; }
        public Guid? EventVenue { get; set; }

        public string EventExamType { get; set; }

        public Guid LearnerIdentifier { get; set; }
        public string LearnerCode { get; set; }
        public string LearnerName { get; set; }
        public string LearnerEmail { get; set; }

        public Guid AssessmentFormIdentifier { get; set; }
        public string AssessmentFormCode { get; set; }
        public string AssessmentFormName { get; set; }
        public string AssessmentFormTitle { get; set; }
    }
}