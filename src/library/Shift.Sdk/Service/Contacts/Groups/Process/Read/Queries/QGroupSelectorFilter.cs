using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QGroupSelectorFilter : Filter
    {
        public string[] GroupTypes { get; private set; }
        public bool ExcludeAdministrators { get; set; }
        public bool MustHavePermissions { get; set; }
        public bool? HasChildren { get; set; }
        public bool? IsEventLocation { get; set; }
        public bool? IsRegistrationEventLocation { get; set; }
        public bool? IsEmployer { get; set; }
        public bool ExcludeDefaultValue { get; set; }
        public Guid[] AlwaysIncludeGroupIdentifiers { get; set; }
        public Guid[] IncludeGroupIdentifiers { get; set; }
        public Guid[] ExcludeGroupIdentifiers { get; set; }
        public Guid? AncestorGroupIdentifier { get; set; }
        public Guid? MembershipUserIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid[] DownstreamUserIdentifiers { get; set; }
        public string AncestorName { get; set; }
        public string DownstreamContactRelationshipType { get; set; }
        public string GrandparentName { get; set; }
        public string GroupLabel { get; set; }
        public string[] GroupNameEndsWithAny { get; set; }
        public string GroupNameStartsWith { get; set; }
        public string ParentName { get; set; }
        public string Keyword { get; set; }

        public string GroupType
        {
            get { return GroupTypes != null && GroupTypes.Length == 1 ? GroupTypes[0] : null; }
            set { GroupTypes = string.IsNullOrEmpty(value) ? null : new[] { value }; }
        }

        public Guid? EmployerContactUserIdentifier { get; set; }

        public bool? OnlyOperatorCanAddUser { get; set; }

        public QGroupSelectorFilter Clone()
        {
            return (QGroupSelectorFilter)MemberwiseClone();
        }
    }
}
