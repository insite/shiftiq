using System;
using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Payments.Read;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    public class QRegistration
    {
        public Guid EventIdentifier { get; set; }
        public Guid? AttemptIdentifier { get; set; }
        public Guid CandidateIdentifier { get; set; }
        public Guid? CustomerIdentifier { get; set; }
        public Guid? EmployerIdentifier { get; set; }
        public Guid? ExamFormIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid? SchoolIdentifier { get; set; }
        public Guid? SeatIdentifier { get; set; }
        public Guid? PaymentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? RegistrationRequestedBy { get; set; }

        public string EventPotentialConflicts { get; set; }
        public string ApprovalProcess { get; set; }
        public string ApprovalReason { get; set; }
        public string ApprovalStatus { get; set; }
        public string AttendanceStatus { get; set; }
        public string CandidateType { get; set; }
        public string BillingCustomer { get; set; }
        public string EligibilityProcess { get; set; }
        public string EligibilityStatus { get; set; }
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
        public string BillingCode { get; set; }

        public bool? MaterialsIncludeDiagramBook { get; set; }

        public int? ExamTimeLimit { get; set; }
        public int? RegistrationSequence { get; set; }
        public int? WorkBasedHoursToDate { get; set; }

        public decimal? RegistrationFee { get; set; }
        public decimal? Score { get; set; }

        public DateTimeOffset? DistributionExpected { get; set; }
        public DateTimeOffset? GradeAssigned { get; set; }
        public DateTimeOffset? GradePublished { get; set; }
        public DateTimeOffset? GradeReleased { get; set; }
        public DateTimeOffset? GradeWithheld { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? RegistrationRequestedOn { get; set; }
        public DateTimeOffset? AttendanceTaken { get; set; }
        public DateTimeOffset? EligibilityUpdated { get; set; }

        public bool IncludeInT2202 { get; set; }

        public virtual QEvent Event { get; set; }
        public virtual QAttempt Attempt { get; set; }
        public virtual QBankForm Form { get; set; }
        public virtual VPerson Candidate { get; set; }
        public virtual QSeat Seat { get; set; }
        public virtual VGroup Employer { get; set; }
        public virtual VGroup Customer { get; set; }
        public virtual QPayment Payment { get; set; }
        public virtual VPerson RegistrationRequestedByPerson { get; set; }

        public virtual ICollection<QRegistrationInstructor> RegistrationInstructors { get; set; } = new HashSet<QRegistrationInstructor>();
        public virtual ICollection<QAccommodation> Accommodations { get; set; } = new HashSet<QAccommodation>();
        public virtual ICollection<QAttempt> Attempts { get; set; } = new HashSet<QAttempt>();

        public bool IsPresent => StringHelper.EqualsAny(AttendanceStatus, new[] { "Attended", "Present" });

        public QRegistration()
        {
            RegistrationInstructors = new HashSet<QRegistrationInstructor>();
            Accommodations = new HashSet<QAccommodation>();
            Attempts = new HashSet<QAttempt>();
        }
    }
}
