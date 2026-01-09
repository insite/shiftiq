using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IEventCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementIdentifier { get; set; }
        Guid? LearnerRegistrationGroupIdentifier { get; set; }
        Guid? MandatorySurveyFormIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? VenueCoordinatorIdentifier { get; set; }
        Guid? VenueLocationIdentifier { get; set; }
        Guid? VenueOfficeIdentifier { get; set; }

        bool? AllowMultipleRegistrations { get; set; }
        bool? AllowRegistrationWithLink { get; set; }
        bool? IntegrationWithholdDistribution { get; set; }
        bool? IntegrationWithholdGrades { get; set; }
        bool? PersonCodeIsRequired { get; set; }
        bool? WaitlistEnabled { get; set; }

        string AppointmentType { get; set; }
        string Content { get; set; }
        string DistributionCode { get; set; }
        string DistributionErrors { get; set; }
        string DistributionProcess { get; set; }
        string DistributionStatus { get; set; }
        string DurationUnit { get; set; }
        string EventBillingType { get; set; }
        string EventCalendarColor { get; set; }
        string EventClassCode { get; set; }
        string EventDescription { get; set; }
        string EventFormat { get; set; }
        string EventPublicationStatus { get; set; }
        string EventRequisitionStatus { get; set; }
        string EventSchedulingStatus { get; set; }
        string EventSource { get; set; }
        string EventSummary { get; set; }
        string EventTitle { get; set; }
        string EventType { get; set; }
        string ExamMaterialReturnShipmentCode { get; set; }
        string ExamMaterialReturnShipmentCondition { get; set; }
        string ExamType { get; set; }
        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string PublicationErrors { get; set; }
        string RegistrationFields { get; set; }
        string VenueRoom { get; set; }

        int? CapacityMaximum { get; set; }
        int? CapacityMinimum { get; set; }
        int? DurationQuantity { get; set; }
        int? EventNumber { get; set; }
        int? ExamDurationInMinutes { get; set; }
        int? InvigilatorMinimum { get; set; }

        decimal? CreditHours { get; set; }

        DateTimeOffset? DistributionExpected { get; set; }
        DateTimeOffset? DistributionOrdered { get; set; }
        DateTimeOffset? DistributionShipped { get; set; }
        DateTimeOffset? DistributionTracked { get; set; }
        DateTimeOffset? EventScheduledEnd { get; set; }
        DateTimeOffset? EventScheduledStart { get; set; }
        DateTimeOffset? ExamMaterialReturnShipmentReceived { get; set; }
        DateTimeOffset? ExamStarted { get; set; }
        DateTimeOffset? LastChangeTime { get; set; }
        DateTimeOffset? RegistrationDeadline { get; set; }
        DateTimeOffset? RegistrationLocked { get; set; }
        DateTimeOffset? RegistrationStart { get; set; }
    }
}