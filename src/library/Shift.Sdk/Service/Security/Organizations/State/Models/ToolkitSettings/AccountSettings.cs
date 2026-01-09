using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AccountSettings
    {
        public Guid? AutomaticGroupJoin { get; set; }

        public bool DisplayDashboardPrototype { get; set; }

        public PersonCodeAutoincrementSettings PersonCodeAutoincrement { get; set; }

        public AccountSettings()
        {
            PersonCodeAutoincrement = new PersonCodeAutoincrementSettings();
        }

        public bool IsShallowEqual(AccountSettings other)
        {
            return AutomaticGroupJoin == other.AutomaticGroupJoin && DisplayDashboardPrototype == other.DisplayDashboardPrototype;
        }
    }
}
