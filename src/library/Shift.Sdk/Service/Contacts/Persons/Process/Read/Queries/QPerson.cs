using System;
using System.Collections.Generic;

using InSite.Application.Organizations.Read;
using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    public class QPerson
    {
        public virtual ICollection<QPersonSecret> Secrets { get; set; } = new HashSet<QPersonSecret>();

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
        public string EmployeeType { get; set; }
        public string EmployeeUnion { get; set; }
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
        public bool MarketingEmailEnabled { get; set; } = PersonState.Defaults.MarketingEmailEnabled;
        public bool IsAdministrator { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsLearner { get; set; }
        public bool IsOperator { get; set; }

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
        public DateTimeOffset? WhenArchived { get; set; }
        public DateTimeOffset? WhenUnarchived { get; set; }
        public DateTimeOffset? SinModified { get; set; }

        public DateTime? Birthdate { get; set; }
        public DateTime? ImmigrationLandingDate { get; set; }
        public DateTime? MemberEndDate { get; set; }
        public DateTime? MemberStartDate { get; set; }

        public virtual QPersonAddress BillingAddress { get; set; }
        public virtual QPersonAddress HomeAddress { get; set; }
        public virtual QPersonAddress ShippingAddress { get; set; }
        public virtual QPersonAddress WorkAddress { get; set; }

        public virtual QUser User { get; set; }
        public virtual QGroup EmployerGroup { get; set; }
        public virtual QOrganization Organization { get; set; }

        public QPersonAddress GetAddress(string addressType)
        {
            if (string.Equals(addressType, ContactAddressType.Billing, StringComparison.OrdinalIgnoreCase))
                return BillingAddress ?? (BillingAddress = new QPersonAddress { });

            if (string.Equals(addressType, ContactAddressType.Home, StringComparison.OrdinalIgnoreCase))
                return HomeAddress ?? (HomeAddress = new QPersonAddress { });

            if (string.Equals(addressType, ContactAddressType.Shipping, StringComparison.OrdinalIgnoreCase))
                return ShippingAddress ?? (ShippingAddress = new QPersonAddress { });

            if (string.Equals(addressType, ContactAddressType.Work, StringComparison.OrdinalIgnoreCase))
                return WorkAddress ?? (WorkAddress = new QPersonAddress { });

            throw new ArgumentException($"Unknown address type: {addressType}");
        }

        public static readonly ICollection<string> DiffExclusions = new HashSet<string>
        {
            nameof(CreatedBy),
            nameof(ModifiedBy),
            nameof(Created),
            nameof(Modified),
            nameof(ShippingAddressIdentifier),
            nameof(BillingAddressIdentifier),
            nameof(WorkAddressIdentifier),
            nameof(HomeAddressIdentifier)
        };
    }
}
