using System;

using Shift.Common;

using Shift.Constant;
using Shift.Constant.Enumerations;

namespace InSite.Persistence
{
    [Serializable]
    public class PersonFilter : Filter
    {
        public PersonFilter()
        {
            NameFilterType = "Similar";
        }

        public Guid? OrganizationIdentifier { get; set; }
        public Guid? OrganizationOrParentIdentifier { get; set; }

        public string[] OrganizationPersonTypes { get; set; }

        public bool MustHaveComments { get; set; }
        public bool MustHaveCompletedCases { get; set; }
        public bool? EmailEnabled { get; set; }
        public bool? EmailAlternateEnabled { get; set; }
        public bool? EmailOrEmailAlternateEnabled { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsJobsApproved { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsCmds { get; set; }
        public bool? IsMultiOrganization { get; set; }
        public bool? IsPasswordAssigned { get; set; }
        public bool? IsConsentToShare { get; set; }
        public bool? IsAdministrator { get; set; }

        public DateTime? UtcCreatedSince { get; set; }
        public DateTime? UtcCreatedBefore { get; set; }
        public DateTime? ModifiedSince { get; set; }
        public DateTime? ModifiedBefore { get; set; }

        public DateTimeOffset? GroupMembershipDate { get; set; }
        public DateTimeOffset? LastAuthenticatedSince { get; set; }
        public DateTimeOffset? LastAuthenticatedBefore { get; set; }
        public DateTimeOffset? IssueStatusEffectiveSince { get; set; }

        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? EmployerParentGroupIdentifier { get; set; }
        public Guid? ExcludeGroupIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? ValidAchievementIdentifier { get; set; }
        public Guid? OccupationStandardIdentifier { get; set; }
        public Guid? OccupationInterest { get; set; }

        public int? SessionCount { get; set; }

        public Guid[] DownstreamUserIdentifiers { get; set; }
        public Guid[] UpstreamUserIdentifiers { get; set; }
        public Guid[] GroupRoleIdentifiers { get; set; }
        public Guid[] GroupDepartmentIdentifiers { get; set; }
        public string[] GroupDepartmentFunctions { get; set; }

        public Guid[] ExcludeUserIdentifiers { get; set; }
        public Guid[] IncludeUserIdentifiers { get; set; }
        public Guid? UserIdentifier
        {
            get => IncludeUserIdentifiers != null && IncludeUserIdentifiers.Length == 1 ? IncludeUserIdentifiers[0] : (Guid?)null;
            set => IncludeUserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }

        public Guid[] AccountStatuses { get; set; }

        public string[] AddressTypes { get; set; }
        public string[] Cities { get; set; }
        public string[] Provinces { get; set; }
        public string[] Street1 { get; set; }

        public string EmailContains { get; set; }
        public string EmailAlternateContains { get; set; }
        public string EmailDomains { get; set; }
        public string EmailStatus { get; set; }
        public string EmailExact
        {
            get => EmailsExact != null && EmailsExact.Length == 1 ? EmailsExact[0] : null;
            set => EmailsExact = value.IsNotEmpty() ? new[] { value } : null;
        }
        public string[] EmailsExact { get; set; }
        public string EmailNotEndsWith { get; set; }
        public bool? IsEmailPatternValid { get; set; }

        public string CodeContains { get; set; }
        public string[] CodesExact { get; set; }
        public string CodeExact
        {
            get => CodesExact != null && CodesExact.Length == 1 ? CodesExact[0] : null;
            set => CodesExact = value.IsNotEmpty() ? new[] { value } : null;
        }
        public string CodeNotExact { get; set; }

        public string CommentKeyword { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public string LastName { get; set; }
        public string NameFilterType { get; set; }
        public string NameOrAccountNumber { get; set; }
        public string Phone { get; set; }
        public string Region { get; set; }
        public string IssueType { get; set; }

        public Guid? ExcludeContainerIdentifier { get; set; }

        public InclusionType CloakedUsers { get; set; }

        public PersonCaseType PersonIssue { get; set; }
        public bool? CandidateIsActivelySeeking { get; set; }
        public string CandidateOccupationKey { get; set; }
        public DateTime? DayLastActive { get; set; }
        public bool? IsCandidate { get; set; }
        public Guid[] EmployerGroups { get; set; }
        public Guid[] Groups { get; set; }

        public int[] CommentsFlag { get; set; }

        public DateTimeOffset? MembershipReasonExpirySince { get; set; }

        public PersonFilter Clone()
        {
            return (PersonFilter)MemberwiseClone();
        }
    }
}
