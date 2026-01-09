using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountEvents : Query<int>, IEventCriteria
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? LearnerRegistrationGroupIdentifier { get; set; }
        public Guid? MandatorySurveyFormIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? VenueCoordinatorIdentifier { get; set; }
        public Guid? VenueLocationIdentifier { get; set; }
        public Guid? VenueOfficeIdentifier { get; set; }

        public bool? AllowMultipleRegistrations { get; set; }
        public bool? AllowRegistrationWithLink { get; set; }
        public bool? IntegrationWithholdDistribution { get; set; }
        public bool? IntegrationWithholdGrades { get; set; }
        public bool? PersonCodeIsRequired { get; set; }
        public bool? WaitlistEnabled { get; set; }

        public string AppointmentType { get; set; }
        public string Content { get; set; }
        public string DistributionCode { get; set; }
        public string DistributionErrors { get; set; }
        public string DistributionProcess { get; set; }
        public string DistributionStatus { get; set; }
        public string DurationUnit { get; set; }
        public string EventBillingType { get; set; }
        public string EventCalendarColor { get; set; }
        public string EventClassCode { get; set; }
        public string EventDescription { get; set; }
        public string EventFormat { get; set; }
        public string EventPublicationStatus { get; set; }
        public string EventRequisitionStatus { get; set; }
        public string EventSchedulingStatus { get; set; }
        public string EventSource { get; set; }
        public string EventSummary { get; set; }
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public string ExamMaterialReturnShipmentCode { get; set; }
        public string ExamMaterialReturnShipmentCondition { get; set; }
        public string ExamType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string PublicationErrors { get; set; }
        public string RegistrationFields { get; set; }
        public string VenueRoom { get; set; }

        public int? CapacityMaximum { get; set; }
        public int? CapacityMinimum { get; set; }
        public int? DurationQuantity { get; set; }
        public int? EventNumber { get; set; }
        public int? ExamDurationInMinutes { get; set; }
        public int? InvigilatorMinimum { get; set; }

        public decimal? CreditHours { get; set; }

        public DateTimeOffset? DistributionExpected { get; set; }
        public DateTimeOffset? DistributionOrdered { get; set; }
        public DateTimeOffset? DistributionShipped { get; set; }
        public DateTimeOffset? DistributionTracked { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public DateTimeOffset? EventScheduledStart { get; set; }
        public DateTimeOffset? ExamMaterialReturnShipmentReceived { get; set; }
        public DateTimeOffset? ExamStarted { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? RegistrationDeadline { get; set; }
        public DateTimeOffset? RegistrationLocked { get; set; }
        public DateTimeOffset? RegistrationStart { get; set; }
    }
}