using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationAccountSettings : Command, IHasRun
    {
        public AccountSettings Accounts { get; set; }

        public ModifyOrganizationAccountSettings(Guid organizationId, AccountSettings accounts)
        {
            AggregateIdentifier = organizationId;
            Accounts = accounts;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var isAccountsSame = state.Toolkits.Accounts.IsShallowEqual(Accounts);
            var isAutoincrementSame = state.Toolkits.Accounts.PersonCodeAutoincrement.IsShallowEqual(Accounts.PersonCodeAutoincrement);

            if (isAccountsSame && isAutoincrementSame)
                return true;

            if (isAutoincrementSame)
                Accounts.PersonCodeAutoincrement = null;

            aggregate.Apply(new OrganizationAccountSettingsModified(Accounts));

            return true;
        }
    }
}
