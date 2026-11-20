using System;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class ContactSettings
    {
        public string FullNamePolicy { get; set; }

        public bool DefaultMFA { get; set; }
        public bool DisableLeaderRelationshipCreation { get; set; }
        public bool PortalSearchActiveMembershipReasons { get; set; }
        public bool ReadOnlyEmploymentDetails { get; set; }
        public bool EnableOperatorGroup { get; set; }
        public bool EnableTraineeDepartment { get; set; }
        public bool PortalSearchRequiresReferral { get; set; }
        public Guid? ProfileSecurityGroupId { get; set; }
        public bool DisplayIntegrationPortalLink { get; set; }
        public string[] ImportReportGroupNames { get; set; }
        public string[] IntegrationPortalGroupNames { get; set; }
        public string IntegrationPortalEmployerGroupName { get; set; }

        public bool IsEqual(ContactSettings other)
        {
            return
                FullNamePolicy.NullIfEmpty() == other.FullNamePolicy.NullIfEmpty() &&
                DefaultMFA == other.DefaultMFA &&
                PortalSearchActiveMembershipReasons == other.PortalSearchActiveMembershipReasons &&
                ReadOnlyEmploymentDetails == other.ReadOnlyEmploymentDetails &&
                DisableLeaderRelationshipCreation == other.DisableLeaderRelationshipCreation &&
                EnableOperatorGroup == other.EnableOperatorGroup &&
                EnableTraineeDepartment == other.EnableTraineeDepartment &&
                PortalSearchRequiresReferral == other.PortalSearchRequiresReferral &&
                ProfileSecurityGroupId == other.ProfileSecurityGroupId &&
                DisplayIntegrationPortalLink == other.DisplayIntegrationPortalLink &&
                GroupsEqual(ImportReportGroupNames, other.ImportReportGroupNames) &&
                GroupsEqual(IntegrationPortalGroupNames, other.IntegrationPortalGroupNames) &&
                IntegrationPortalEmployerGroupName.NullIfEmpty() == other.IntegrationPortalEmployerGroupName.NullIfEmpty()
                ;
        }

        private static bool GroupsEqual(string[] groups1, string[] groups2)
        {
            var groupsOrEmpty1 = groups1.EmptyIfNull();
            var groupsOrEmpty2 = groups2.EmptyIfNull();
            return groupsOrEmpty1.Length == groupsOrEmpty2.Length
                && groupsOrEmpty1.Zip(groupsOrEmpty2, (a, b) => string.Equals(a, b)).All(x => x);
        }
    }
}
