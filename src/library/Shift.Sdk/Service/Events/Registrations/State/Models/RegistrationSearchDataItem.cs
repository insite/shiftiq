using System;
using System.Collections.Generic;

namespace InSite.Domain.Registrations
{
    public class RegistrationSearchDataItem
    {
        public DateTimeOffset EventScheduledStart { get; set; }
        public DateTimeOffset? EventScheduledEnd { get; set; }
        public string EventType { get; set; }
        public Guid EventIdentifier { get; set; }
        public string EventTitle { get; set; }
        public string EventAchievementTitle { get; set; }
        public string EventAchievementDescription { get; set; }
        public DateTimeOffset? RegistrationRequestedOn { get; set; }
        public Guid CandidateIdentifier { get; set; }
        public string CandidateFullName { get; set; }
        public string CandidatePersonCode { get; set; }
        public string CandidateFirstLanguage { get; set; }
        public string CandidatePostalCode { get; set; }
        public string ApprovalStatus { get; set; }
        public string AttendanceStatus { get; set; }
        public decimal? RegistrationFee { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public string CandidateEmail { get; set; }
        public bool? CandidateEmailEnabled { get; set; }
        public string EmployerGroupName { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public string EmployerGroupRegion { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string CandidatePhone { get; set; }
        public string LearnerId { get; set; }
        public int? RegistrationSequence { get; set; }
        public decimal? WorkBasedHoursToDate { get; set; }
        public string RegistrationComment { get; set; }
        public bool IncludeInT2202 { get; set; }
        public string PaymentStatus { get; set; }
        public Guid? RegistrationRequestedByIdentifier { get; set; }
        public string RegistrationRequestedByName { get; set; }
        public string RegistrationRequestedByEmail { get; set; }
        public string BillingCode { get; set; }
        public string Department { get; set; }
        public string ExamFormTitle { get; set; }
        public string ExamFormName { get; set; }
        public string ExamFormCode { get; set; }
        public IEnumerable<string> DepartmentNames { get; set; }
    }
}
