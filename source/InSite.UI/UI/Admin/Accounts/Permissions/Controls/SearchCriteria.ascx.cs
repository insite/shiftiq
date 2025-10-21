using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TGroupActionFilter>
    {
        public override TGroupActionFilter Filter
        {
            get
            {
                var filter = new TGroupActionFilter
                {
                    ActionIdentifier = ActionIdentifier.Value,
                    GroupIdentifier = GroupIdentifier.Value,
                    OrganizationIdentifier = Organization.Identifier
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ActionIdentifier.Value = value.ActionIdentifier;
                if (value.GroupIdentifier.HasValue)
                    GroupIdentifier.Value = value.GroupIdentifier;
            }
        }

        public override void Clear()
        {
            ActionIdentifier.Value = null;
            GroupType.ClearSelection();
            GroupIdentifier.Value = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += GroupType_ValueChanged;
        }

        private void GroupType_ValueChanged(object sender, EventArgs e)
        {
            SetupGroupType();
        }

        private void SetupGroupType()
        {
            GroupIdentifier.Filter.GroupType = GroupType.Value;
            GroupIdentifier.Value = null;
        }
    }
}