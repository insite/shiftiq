using System;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    [Serializable]
    public class QRegistrationFilter : Filter
    {
        public Guid? RegistrationIdentifier
        {
            get => RegistrationIdentifiers != null && RegistrationIdentifiers.Length == 1 ? RegistrationIdentifiers[0] : (Guid?)null;
            set => RegistrationIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] RegistrationIdentifiers { get; set; }
        public Guid[] CandidateIdentifiers { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? OccupationIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }

        public string Event { get; set; }
        public Guid? EventIdentifier { get; set; }
        public string EventFormat { get; set; }
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public DateTimeOffset? EventScheduledSince { get; set; }
        public DateTimeOffset? EventScheduledBefore { get; set; }
        public bool? HasEvent { get; set; }
        public string AttendanceStatus { get; set; }
        public string[] AttendanceStatuses { get; set; }
        public string ExamFormName { get; set; }
        public string BillingCode { get; set; }

        public Guid? BankIdentifier { get; set; }

        public string Form { get; set; }
        public string FormName { get; set; }

        public Guid? FormIdentifier
        {
            get => FormIdentifiers != null && FormIdentifiers.Length == 1 ? FormIdentifiers[0] : (Guid?)null;
            set => FormIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }

        public Guid[] FormIdentifiers { get; set; }

        public bool? HasExamForm { get; set; }

        public Guid? QuestionIdentifier { get; set; }

        public string Candidate { get; set; }
        public string CandidateCode { get; set; }
        public string CandidateCompany { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public string CandidateType { get; set; }
        public Guid? CandidateIdentifier { get; set; }
        public bool? HasCandidate { get; set; }
        public bool? CandidateEmailEnabled { get; set; }

        public string SubmissionGrade { get; set; }
        public string[] SubmissionTag { get; set; }
        public int? SubmissionScoreFrom { get; set; }
        public int? SubmissionScoreThru { get; set; }
        public DateTimeOffset? AttemptCompletedSince { get; set; }
        public DateTimeOffset? AttemptCompletedBefore { get; set; }

        public bool IsRegistered { get; set; }
        public string ApprovalStatus { get; set; }
        public string[] ApprovalStatuses { get; set; }
        public string GradingStatus { get; set; }
        public DateTimeOffset? RegistrationStartedSince { get; set; }
        public DateTimeOffset? RegistrationStartedBefore { get; set; }
        public DateTimeOffset? RegistrationCompletedSince { get; set; }
        public DateTimeOffset? RegistrationCompletedBefore { get; set; }
        public DateTimeOffset? RegistrationRequestedSince { get; set; }
        public DateTimeOffset? RegistrationRequestedBefore { get; set; }
        public bool? IsScheduled { get; set; }
        public bool? IsStarted { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsAttendanceOverdue { get; set; }
        public bool? IsWithheld { get; set; }
        public Guid? CandidateEmployerGroupIdentifier { get; set; }
        public bool? SeatAvailable { get; set; }
        public Guid? SeatIdentifier { get; set; }
        public string RegistrationEmployerName { get; set; }
        public string RegistrationEmployerStatus { get; set; }
        public string RegistrationEmployerRegion { get; set; }
        public string RegistrationCustomerName { get; set; }
        public string VenueName { get; set; }
        public Guid? RegistrationEmployerIdentifier { get; set; }
        public Guid? RegistrationCustomerIdentifier { get; set; }
        public string RegistrationComment { get; set; }
        public bool? IncludeInT2202 { get; set; }
        public string PaymentStatus { get; set; }
        public Guid? PaymentIdentifier { get; set; }
        public Guid[] VenueLocationIdentifier { get; set; }
        public Guid? RegistrationRequestedBy { get; set; }
        public string RegistrationRequestedByName { get; set; }
        public Guid[] ExcludeCandidateIdentifier { get; set; }
        public Guid? CandidateMembershipGroupIdentifier { get; set; }

        public QRegistrationFilter Clone()
        {
            return (QRegistrationFilter)MemberwiseClone();
        }
    }
}
