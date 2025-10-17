using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QPersonFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? OrganizationOrParentOrganizationIdentifier { get; set; }
        public Guid? UserIdentifier
        {
            get => UserIdentifiers != null && UserIdentifiers.Length == 1 ? UserIdentifiers[0] : (Guid?)null;
            set => UserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] UserIdentifiers { get; set; }
        public Guid? ExcludeUserIdentifier
        {
            get => ExcludeUserIdentifiers != null && ExcludeUserIdentifiers.Length == 1 ? ExcludeUserIdentifiers[0] : (Guid?)null;
            set => ExcludeUserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] ExcludeUserIdentifiers { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? UserMembershipGroupIdentifier { get; set; }
        public string UserMembershipGroupTypeContains { get; set; }
        public string UserMembershipGroupTypeExact { get; set; }
        public string UserMembershipGroupLabelContains { get; set; }
        public string UserMembershipGroupLabelExact { get; set; }
        public string UserNameContains { get; set; }
        public string UserNameExact { get; set; }
        public string UserEmailContains { get; set; }
        public string UserEmailExact { get; set; }
        public bool IsNeedReview { get; set; }
        public string[] PersonCodes { get; set; }
        public string PersonCode
        {
            get => PersonCodes != null && PersonCodes.Length == 1 ? PersonCodes[0] : null;
            set => PersonCodes = value.IsNotEmpty() ? new[] { value } : null;
        }
        public string UserNameOrPersonCodeContains { get; set; }
        public bool? HasPersonCode { get; set; }

        public bool? IsAdministrator { get; set; }

        public QPersonFilter Clone()
        {
            return (QPersonFilter)MemberwiseClone();
        }
    }
}
