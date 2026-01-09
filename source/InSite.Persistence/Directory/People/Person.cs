using System;

using Shift.Common;

namespace InSite.Persistence
{
    public class Person : IHasTimestamp
    {
        public Guid? BillingAddressIdentifier { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? HomeAddressIdentifier { get; set; }
        public Guid? IndustryItemIdentifier { get; set; }
        public Guid? MembershipStatusItemIdentifier { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid? OccupationStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid PersonIdentifier { get; set; }
        public Guid? ShippingAddressIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? WorkAddressIdentifier { get; set; }

        public string AccessRevokedBy { get; set; }
        public string AgeGroup { get; set; }
        public string CandidateLinkedInUrl { get; set; }
        public string CandidateOccupationList { get; set; }
        public string Citizenship { get; set; }
        public string ConsentConsultation { get; set; }
        public string ConsentToShare { get; set; }
        public string CredentialingCountry { get; set; }
        public string EducationLevel { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string EmployeeUnion { get; set; }
        public string EmployeeType { get; set; }
        public string FirstLanguage { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ImmigrationApplicant { get; set; }
        public string ImmigrationCategory { get; set; }
        public string ImmigrationDestination { get; set; }
        public string ImmigrationDisability { get; set; }
        public string ImmigrationNumber { get; set; }
        public string JobsApprovedBy { get; set; }
        public string JobDivision { get; set; }
        public string JobTitle { get; set; }
        public string Language { get; set; }
        public string PersonCode { get; set; }
        public string PersonType { get; set; }
        public string Phone { get; set; }
        public string PhoneFax { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneOther { get; set; }
        public string PhoneWork { get; set; }
        public string Referrer { get; set; }
        public string ReferrerOther { get; set; }
        public string Region { get; set; }
        public string ShippingPreference { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string TradeworkerNumber { get; set; }
        public string UserAccessGrantedBy { get; set; }
        public string UserApproveReason { get; set; }
        public string WebSiteUrl { get; set; }

        public bool? CandidateIsActivelySeeking { get; set; }
        public bool? CandidateIsWillingToRelocate { get; set; }
        public bool EmailAlternateEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsArchived { get; set; }
        public bool IsLearner { get; set; }
        public bool IsOperator { get; set; }
        public bool MarketingEmailEnabled { get; set; }

        public int? CandidateCompletionProfilePercent { get; set; }
        public int? CandidateCompletionResumePercent { get; set; }
        public int? CustomKey { get; set; }
        public int? WelcomeEmailsSentToUser { get; set; }

        public DateTimeOffset? AccessRevoked { get; set; }
        public DateTimeOffset? AccountReviewCompleted { get; set; }
        public DateTimeOffset? AccountReviewQueued { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? JobsApproved { get; set; }
        public DateTimeOffset? LastAuthenticated { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset? UserAccessGranted { get; set; }

        public DateTime? Birthdate { get; set; }
        public DateTime? ImmigrationLandingDate { get; set; }
        public DateTime? MemberEndDate { get; set; }
        public DateTime? MemberStartDate { get; set; }

        public virtual VOrganization Organization { get; set; }
        public virtual User User { get; set; }
        public virtual VGroupDetail EmployerGroup { get; set; }
        public virtual Address BillingAddress { get; set; }
        public virtual Address HomeAddress { get; set; }
        public virtual Address ShippingAddress { get; set; }
        public virtual Address WorkAddress { get; set; }
        public virtual TCollectionItem MembershipStatus { get; set; }
        public virtual Standard OccupationStandard { get; set; }
    }
}