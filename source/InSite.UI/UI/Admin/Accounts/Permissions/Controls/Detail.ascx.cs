using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class Detail : UserControl
    {
        private Guid? DefaultActionID
        {
            get { return (Guid?)ViewState[nameof(DefaultActionID)]; }
            set { ViewState[nameof(DefaultActionID)] = value; }
        }

        private Guid? DefaultGroupID
        {
            get { return (Guid?)ViewState[nameof(DefaultGroupID)]; }
            set { ViewState[nameof(DefaultGroupID)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += GroupType_ValueChanged;
        }

        private void GroupType_ValueChanged(object sender, EventArgs e)
        {
            OnGroupTypeChanged();
        }

        private void OnGroupTypeChanged()
        {
            GroupIdentifier.Filter.GroupType = GroupType.Value;
            GroupIdentifier.Value = null;
        }

        public void SetDefaultInputValues(Guid? actionId, Guid? groupId)
        {
            var action = actionId.HasValue ? TActionSearch.Get(actionId.Value) : null;
            if (action != null)
            {
                DefaultActionID = action.ActionIdentifier;
                PermissionIdentifier.Value = DefaultActionID;
            }

            var group = groupId.HasValue ? ServiceLocator.GroupSearch.GetGroup(groupId.Value) : null;
            if (group != null)
            {
                DefaultGroupID = group.GroupIdentifier;
                GroupType.Value = group.GroupType;
                OnGroupTypeChanged();
                GroupIdentifier.Value = group.GroupIdentifier;
            }
            else
            {
                OnGroupTypeChanged();
            }

            PermissionIdentifier.Enabled = !DefaultActionID.HasValue;
            GroupType.Enabled = !DefaultGroupID.HasValue;
            GroupIdentifier.Enabled = !DefaultGroupID.HasValue;
        }

        public void SetInputValues(TAction action, QGroup group)
        {
            DefaultActionID = action.ActionIdentifier;
            DefaultGroupID = group.GroupIdentifier;

            GroupType.Value = group.GroupType;
            OnGroupTypeChanged();
            GroupIdentifier.Value = group.GroupIdentifier;
            
            PermissionIdentifier.Value = action.ActionIdentifier;

            PermissionIdentifier.Enabled = false;
            GroupType.Enabled = false;
            GroupIdentifier.Enabled = false;

            var access = TGroupPermissionSearch.Select(group.GroupIdentifier, action.ActionIdentifier);

            AllowRead.Checked = access.AllowRead;
            AllowWrite.Checked = access.AllowWrite;

            AllowCreate.Checked = access.AllowCreate;
            AllowDelete.Checked = access.AllowDelete;

            AllowAdministrate.Checked = access.AllowAdministrate;
            AllowConfigure.Checked = access.AllowConfigure;

            AllowTrialAccess.Checked = access.AllowTrialAccess;
        }

        public void Save(out Guid permissionId)
        {
            var groupId = DefaultGroupID ?? GroupIdentifier.Value ?? throw new ApplicationError("Group is undefined");
            var actionId = PermissionIdentifier.Value ?? DefaultActionID ?? throw new ApplicationError("Action is undefined");

            permissionId = TGroupPermissionStore.Update(groupId, actionId, "Action", AllowExecute.Checked, AllowRead.Checked, AllowWrite.Checked, AllowCreate.Checked, AllowDelete.Checked, AllowAdministrate.Checked, AllowConfigure.Checked, AllowTrialAccess.Checked);
        }
    }
}