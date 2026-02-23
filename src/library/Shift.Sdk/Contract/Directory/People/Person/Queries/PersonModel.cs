using System;

namespace Shift.Contract
{
    public partial class PersonModel
    {
        // Identity

        public string FullName { get; set; }
        public Guid OrganizationId { get; set; }
        public string PersonCode { get; set; }
        public string PersonFirstName { get; set; }
        public Guid PersonId { get; set; }
        public string PersonLastName { get; set; }
        public string PersonType { get; set; }

        // User Account

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsLearner { get; set; }
        public bool IsOperator { get; set; }
        public DateTimeOffset? AccessRevoked { get; set; }
        public string AccessRevokedBy { get; set; }
        public DateTimeOffset? LastAuthenticated { get; set; }
        public DateTimeOffset? UserAccessGranted { get; set; }
        public string UserAccessGrantedBy { get; set; }
        public string UserApproveReason { get; set; }
        public int? WelcomeEmailsSentToUser { get; set; }

        // Communication Preferences

        public bool EmailAlternateEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool MarketingEmailEnabled { get; set; }

        // Contact / Phones

        public string Phone { get; set; }
        public string PhoneFax { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneOther { get; set; }
        public string PhoneWork { get; set; }

        // Addresses

        public PersonAddressModel BillingAddress { get; set; }
        public PersonAddressModel HomeAddress { get; set; }
        public PersonAddressModel ShippingAddress { get; set; }
        public string ShippingPreference { get; set; }
        public PersonAddressModel WorkAddress { get; set; }

        // Emergency Contact

        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelationship { get; set; }

        // Demographics

        public string AgeGroup { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Citizenship { get; set; }
        public string FirstLanguage { get; set; }
        public string Gender { get; set; }
        public string Language { get; set; }
        public string Region { get; set; }
        public string TimeZone { get; set; }

        // Employment / Job

        public Guid? EmployerGroupId { get; set; }
        public string EmployeeType { get; set; }
        public string EmployeeUnion { get; set; }
        public string JobDivision { get; set; }
        public string JobTitle { get; set; }
        public DateTimeOffset? JobsApproved { get; set; }
        public string JobsApprovedBy { get; set; }
        public Guid? OccupationStandardId { get; set; }

        // Candidate

        public int? CandidateCompletionProfilePercent { get; set; }
        public int? CandidateCompletionResumePercent { get; set; }
        public bool? CandidateIsActivelySeeking { get; set; }
        public bool? CandidateIsWillingToRelocate { get; set; }
        public string CandidateLinkedInUrl { get; set; }
        public string CandidateOccupationList { get; set; }

        // Immigration

        public string ImmigrationApplicant { get; set; }
        public string ImmigrationCategory { get; set; }
        public string ImmigrationDestination { get; set; }
        public string ImmigrationDisability { get; set; }
        public DateTime? ImmigrationLandingDate { get; set; }
        public string ImmigrationNumber { get; set; }

        // Membership

        public DateTime? MemberEndDate { get; set; }
        public Guid? MembershipStatusItemId { get; set; }
        public DateTime? MemberStartDate { get; set; }

        // Education / Credentials

        public string CredentialingCountry { get; set; }
        public string EducationLevel { get; set; }
        public Guid? IndustryItemId { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string TradeworkerNumber { get; set; }

        // Consent

        public string ConsentConsultation { get; set; }
        public string ConsentToShare { get; set; }

        // Referrer

        public string Referrer { get; set; }
        public string ReferrerOther { get; set; }

        // Archive

        public bool IsArchived { get; set; }
        public DateTimeOffset? WhenArchived { get; set; }
        public DateTimeOffset? WhenUnarchived { get; set; }

        // Account Review

        public DateTimeOffset? AccountReviewCompleted { get; set; }
        public DateTimeOffset? AccountReviewQueued { get; set; }

        // Web

        public string WebSiteUrl { get; set; }

        // Audit

        public DateTimeOffset Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public int? CustomKey { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset? SinModified { get; set; }
    }
}
