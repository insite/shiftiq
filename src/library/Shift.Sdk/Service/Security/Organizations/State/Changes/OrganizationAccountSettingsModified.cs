using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationAccountSettingsModified : Change
    {
        public AccountSettings Accounts { get; set; }

        public OrganizationAccountSettingsModified(AccountSettings accounts)
        {
            Accounts = accounts;
        }
    }
}
