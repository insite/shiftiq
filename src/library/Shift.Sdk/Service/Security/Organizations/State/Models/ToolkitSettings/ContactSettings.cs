using System;

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

        public bool IsEqual(ContactSettings other)
        {
            return
                FullNamePolicy.NullIfEmpty() == other.FullNamePolicy.NullIfEmpty() &&
                DefaultMFA == other.DefaultMFA &&
                PortalSearchActiveMembershipReasons == other.PortalSearchActiveMembershipReasons &&
                ReadOnlyEmploymentDetails == other.ReadOnlyEmploymentDetails &&
                DisableLeaderRelationshipCreation == other.DisableLeaderRelationshipCreation &&
                EnableOperatorGroup == other.EnableOperatorGroup &&
                EnableTraineeDepartment == other.EnableTraineeDepartment
                ;
        }
    }
}
