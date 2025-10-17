using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AccountSettings
    {
        public string OrganizationIndustry { get; set; }
        public Guid? AutomaticGroupJoin { get; set; }
        public bool DisplayDashboardPrototype { get; set; }

        public PersonCodeAutoincrementSettings PersonCodeAutoincrement { get; set; }

        public AccountSettings()
        {
            PersonCodeAutoincrement = new PersonCodeAutoincrementSettings();
        }

        public bool IsShallowEqual(AccountSettings other)
        {
            return OrganizationIndustry.NullIfEmpty() == other.OrganizationIndustry.NullIfEmpty()
                && AutomaticGroupJoin == other.AutomaticGroupJoin
                && DisplayDashboardPrototype == other.DisplayDashboardPrototype;
        }
    }
}
