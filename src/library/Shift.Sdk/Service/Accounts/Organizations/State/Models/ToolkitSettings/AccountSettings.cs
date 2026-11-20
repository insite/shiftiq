using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AccountSettings
    {
        public Guid? AutomaticGroupJoin { get; set; }

        public bool DisplayDashboardPrototype { get; set; }

        public bool HideAdminMyDashboard { get; set; }

        public PersonCodeAutoincrementSettings PersonCodeAutoincrement { get; set; }

        public bool OtherEnvironments { get; set; }

        public bool AHRIFeatures { get; set; } // Temporary setting, will be removed later

        public AccountSettings()
        {
            PersonCodeAutoincrement = new PersonCodeAutoincrementSettings();
        }

        public bool IsShallowEqual(AccountSettings other)
        {
            return AutomaticGroupJoin == other.AutomaticGroupJoin
                && DisplayDashboardPrototype == other.DisplayDashboardPrototype
                && HideAdminMyDashboard == other.HideAdminMyDashboard
                && OtherEnvironments == other.OtherEnvironments
                && AHRIFeatures == other.AHRIFeatures
                ;
        }
    }
}
