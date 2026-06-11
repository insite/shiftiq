using System;

namespace Shift.Contract
{
    public class ModifyRegistration
    {
        public Guid? AttemptIdentifier { get; set; }
        public Guid CandidateIdentifier { get; set; }
        public Guid? CustomerIdentifier { get; set; }
        public Guid? EmployerIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }
        public Guid? ExamFormIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PaymentIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid? RegistrationRequestedBy { get; set; }
        public Guid? SchoolIdentifier { get; set; }
        public Guid? SeatIdentifier { get; set; }

        public bool IncludeInT2202 { get; set; }
        public bool? MaterialsIncludeDiagramBook { get; set; }

        public string ApprovalProcess { get; set; }
        public string ApprovalReason { get; set; }
        public string ApprovalStatus { get; set; }
        public string AttendanceStatus { get; set; }
        public string BillingCode { get; set; }
        public string BillingCustomer { get; set; }
        public string CandidateType { get; set; }
        public string EligibilityProcess { get; set; }
        public string EligibilityStatus { get; set; }
        public string EventPotentialConflicts { get; set; }
        public string Grade { get; set; }
        public string GradeWithheldReason { get; set; }
        public string GradingProcess { get; set; }
        public string GradingStatus { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string MaterialsPackagedForDistribution { get; set; }
        public string MaterialsPermittedToCandidates { get; set; }
        public string RegistrationComment { get; set; }
        public string RegistrationPassword { get; set; }
        public string RegistrationSource { get; set; }
        public string SynchronizationProcess { get; set; }
        public string SynchronizationStatus { get; set; }

        public int? ExamTimeLimit { get; set; }
        public int? RegistrationSequence { get; set; }
        public int? WorkBasedHoursToDate { get; set; }

        public decimal? RegistrationFee { get; set; }
        public decimal? Score { get; set; }

        public DateTimeOffset? AttendanceTaken { get; set; }
        public DateTimeOffset? DistributionExpected { get; set; }
        public DateTimeOffset? EligibilityUpdated { get; set; }
        public DateTimeOffset? GradeAssigned { get; set; }
        public DateTimeOffset? GradePublished { get; set; }
        public DateTimeOffset? GradeReleased { get; set; }
        public DateTimeOffset? GradeWithheld { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? RegistrationRequestedOn { get; set; }
    }
}