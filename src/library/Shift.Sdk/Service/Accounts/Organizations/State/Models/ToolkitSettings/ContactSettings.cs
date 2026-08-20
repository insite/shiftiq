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

        public bool IsEqual(ContactSettings other)
        {
            var isEqual =
                FullNamePolicy.NullIfEmpty() == other.FullNamePolicy.NullIfEmpty() &&
                DefaultMFA == other.DefaultMFA &&
                PortalSearchActiveMembershipReasons == other.PortalSearchActiveMembershipReasons &&
                ReadOnlyEmploymentDetails == other.ReadOnlyEmploymentDetails &&
                DisableLeaderRelationshipCreation == other.DisableLeaderRelationshipCreation &&
                EnableOperatorGroup == other.EnableOperatorGroup &&
                EnableTraineeDepartment == other.EnableTraineeDepartment &&
                PortalSearchRequiresReferral == other.PortalSearchRequiresReferral &&
                ProfileSecurityGroupId == other.ProfileSecurityGroupId &&
                DisplayIntegrationPortalLink == other.DisplayIntegrationPortalLink
                ;

            if (!isEqual)
                return false;

            var groups1 = ImportReportGroupNames.EmptyIfNull();
            var groups2 = other.ImportReportGroupNames.EmptyIfNull();
            return groups1.Length == groups2.Length
                && groups1.Zip(groups2, (a, b) => string.Equals(a, b)).All(x => x);
        }
    }
}
