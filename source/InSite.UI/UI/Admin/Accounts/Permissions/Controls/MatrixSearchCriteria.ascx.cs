using System;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class MatrixSearchCriteria : SearchCriteriaController<MatrixFilter>
    {
        public override MatrixFilter Filter
        {
            get
            {
                var filter = new MatrixFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    GroupIdentifier = GroupIdentifier.Value,
                    ActionIdentifier = PermissionIdentifier.Value,
                    GroupType = !IsPostBack ? GroupType.Value.IfNullOrEmpty(GroupTypes.Role) : GroupType.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                GroupType.Value = value.GroupType;

                OnGroupTypeChanged();

                GroupIdentifier.Value = value.GroupIdentifier;

                PermissionIdentifier.Value = value.ActionIdentifier;
            }
        }

        public override void Clear()
        {
            GroupType.Value = GroupTypes.Role;

            OnGroupTypeChanged();

            PermissionIdentifier.Value = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += GroupType_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                OnGroupTypeChanged();
        }

        private void GroupType_ValueChanged(object sender, EventArgs e) => OnGroupTypeChanged();

        private void OnGroupTypeChanged()
        {
            GroupIdentifier.Filter.MustHavePermissions = true;
            GroupIdentifier.Filter.GroupType = GroupType.Value;
            GroupIdentifier.Value = null;
        }
    }
}