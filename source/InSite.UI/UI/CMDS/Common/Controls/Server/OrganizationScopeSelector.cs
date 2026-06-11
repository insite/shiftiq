using System;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    /// <summary>
    /// Selects which organization a search targets: the current organization (default),
    /// the shared global (partition) organization, or both. Centralizes the option list,
    /// the scope constants, and the resolution of the selected scope into the set of
    /// owning organization identifiers to filter on, so consumers stay in sync.
    /// </summary>
    public class OrganizationScopeSelector : ComboBox
    {
        public const string ScopeOrganization = AccountScopes.Organization;
        public const string ScopePartition = AccountScopes.Partition;
        public const string ScopeBoth = "Both";

        protected override BindingType ControlBinding => BindingType.Code;

        // Default to a required selection; the first option (current organization) is the default.
        public override bool AllowBlank
        {
            get => (bool)(ViewState[nameof(AllowBlank)] ?? false);
            set => ViewState[nameof(AllowBlank)] = value;
        }

        // Whether the combined "This Organization and Global" item is offered. Default true.
        public bool IsCombinedScopeVisible
        {
            get => ViewState[nameof(IsCombinedScopeVisible)] == null || (bool)ViewState[nameof(IsCombinedScopeVisible)];
            set => ViewState[nameof(IsCombinedScopeVisible)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(ScopeOrganization, "This Organization");
            list.Add(ScopePartition, "Global");

            if (IsCombinedScopeVisible)
                list.Add(ScopeBoth, "This Organization and Global");

            return list;
        }

        /// <summary>
        /// Resolves the selected scope into the set of owning organization identifiers to filter on,
        /// or null for the default (current organization) scope, which leaves existing filtering intact.
        /// </summary>
        public Guid[] ResolveOwnerOrganizationIdentifiers(Guid organizationIdentifier, Guid partitionIdentifier)
        {
            switch (Value)
            {
                case ScopePartition:
                    return new[] { partitionIdentifier };

                case ScopeBoth:
                    return new[] { organizationIdentifier, partitionIdentifier };

                default:
                    return null;
            }
        }
    }
}
