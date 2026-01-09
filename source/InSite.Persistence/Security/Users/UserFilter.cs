using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class UserFilter : Filter
    {
        public string EmailContains { get; set; }
        public string EmailExact { get; set; }
        public string EmailAlternateExact { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? IsEmailValid { get; set; }
        public bool? IsAccessGranted { get; set; }
        public bool? IsLicensed { get; set; }
        public string ContactName { get; set; }
        public bool IncludeNullUserName { get; set; }
        public string CompanyName { get; set; }
        public string OrganizationStatus { get; set; }
        public string UserSessionStatus { get; set; }
        public bool? IsCmds { get; set; }

        public Guid[] ExcludeUserIdentifiers { get; set; }
        public Guid[] IncludeUserIdentifiers { get; set; }
        public Guid? UserIdentifier
        {
            get => IncludeUserIdentifiers != null && IncludeUserIdentifiers.Length == 1 ? IncludeUserIdentifiers[0] : (Guid?)null;
            set => IncludeUserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }

        public Guid[] PersonOrganizationIdentifiers { get; set; }
        public Guid? PersonOrganizationIdentifier
        {
            get => PersonOrganizationIdentifiers != null && PersonOrganizationIdentifiers.Length == 1 ? PersonOrganizationIdentifiers[0] : (Guid?)null;
            set => PersonOrganizationIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public bool? PersonEmailEnabled { get; set; }

        public Guid? MembershipGroupIdentifier { get; set; }
        public string MembershipGroupName { get; set; }
        public string MembershipType { get; set; }
        public bool MembershipTypeAnd { get; set; }

        public DateTimeOffsetRange LastAuthenticated { get; set; }
        public DateTimeOffsetRange AccessGranted { get; set; }
        public DateTimeOffsetRange DefaultPasswordExpired { get; set; }

        /// <summary>
        /// This is the organization for the session in which a user or system is executing a search with this filter.
        /// </summary>
        public Guid? AgentOrganizationIdentifier { get; set; }

        public UserFilter()
        {
            LastAuthenticated = new DateTimeOffsetRange();
            AccessGranted = new DateTimeOffsetRange();
            DefaultPasswordExpired = new DateTimeOffsetRange();
        }
    }
}