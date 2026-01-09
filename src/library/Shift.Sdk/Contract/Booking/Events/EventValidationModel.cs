using System;

using Shift.Constant;

namespace Shift.Sdk.Contract.Booking
{
    public class EventValidationModel
    {
        public string ApprovalStatus { get; set; }
        public bool AllowLoginAnyTime { get; set; }
        public bool HasEvent { get; set; }
        public string EventFormat { get; set; }
        public string EventSchedulingStatus { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public Guid? ExamFormIdentifier { get; set; }
        public bool HasForm { get; set; }
        public int AssessmentCount { get; set; }
        public bool HasAccommodations { get; set; }
        public bool HasResumeInterruptedOnlineExam { get; set; }
        public string UserTimeZone { get; set; }
        public EventClassStatus? ClassStatus { get; set; }

        public EventValidationModel Clone() => (EventValidationModel)MemberwiseClone();
    }
}