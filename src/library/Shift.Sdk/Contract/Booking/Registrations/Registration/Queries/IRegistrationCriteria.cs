using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRegistrationCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? AttemptIdentifier { get; set; }
        Guid? CandidateIdentifier { get; set; }
        Guid? CustomerIdentifier { get; set; }
        Guid? EmployerIdentifier { get; set; }
        Guid? EventIdentifier { get; set; }
        Guid? ExamFormIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? PaymentIdentifier { get; set; }
        Guid? RegistrationRequestedBy { get; set; }
        Guid? SchoolIdentifier { get; set; }
        Guid? SeatIdentifier { get; set; }

        bool? IncludeInT2202 { get; set; }
        bool? MaterialsIncludeDiagramBook { get; set; }

        string ApprovalProcess { get; set; }
        string ApprovalReason { get; set; }
        string ApprovalStatus { get; set; }
        string AttendanceStatus { get; set; }
        string BillingCode { get; set; }
        string BillingCustomer { get; set; }
        string CandidateType { get; set; }
        string EligibilityProcess { get; set; }
        string EligibilityStatus { get; set; }
        string EventPotentialConflicts { get; set; }
        string Grade { get; set; }
        string GradeWithheldReason { get; set; }
        string GradingProcess { get; set; }
        string GradingStatus { get; set; }
        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string MaterialsPackagedForDistribution { get; set; }
        string MaterialsPermittedToCandidates { get; set; }
        string RegistrationComment { get; set; }
        string RegistrationPassword { get; set; }
        string RegistrationSource { get; set; }
        string SynchronizationProcess { get; set; }
        string SynchronizationStatus { get; set; }

        int? ExamTimeLimit { get; set; }
        int? RegistrationSequence { get; set; }
        int? WorkBasedHoursToDate { get; set; }

        decimal? RegistrationFee { get; set; }
        decimal? Score { get; set; }

        DateTimeOffset? AttendanceTaken { get; set; }
        DateTimeOffset? DistributionExpected { get; set; }
        DateTimeOffset? EligibilityUpdated { get; set; }
        DateTimeOffset? GradeAssigned { get; set; }
        DateTimeOffset? GradePublished { get; set; }
        DateTimeOffset? GradeReleased { get; set; }
        DateTimeOffset? GradeWithheld { get; set; }
        DateTimeOffset? LastChangeTime { get; set; }
        DateTimeOffset? RegistrationRequestedOn { get; set; }
    }
}