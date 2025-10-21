using System;
using System.Web.UI;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Utilities.Actions.Controls
{
    public partial class Detail : UserControl
    {
        private Guid ActionIdentifier
        {
            get => (Guid)ViewState[nameof(ActionIdentifier)];
            set => ViewState[nameof(ActionIdentifier)] = value;
        }

        public void SetInputValues(TAction info)
        {
            ActionIdentifier = info.ActionIdentifier;

            if (info.PermissionParentActionIdentifier.HasValue)
            {
                var permission = TActionSearch.Get(info.PermissionParentActionIdentifier.Value);
                if (permission != null)
                    PermissionIdentifier.Value = permission.ActionIdentifier;
            }

            NavigationBack.Filter.ExcludeActionIdentifier = info.ActionIdentifier;
            NavigationBack.Value = info.NavigationParentActionIdentifier;

            NavigationForwardRepeater.DataSource = info.NavigationChildren;
            NavigationForwardRepeater.DataBind();
            NavigationForwardField.Visible = info.NavigationChildren.Count > 0;

            ActionList.Text = info.ActionList;

            var backUrl = string.Empty;
            if (info.NavigationParentActionIdentifier != null)
                backUrl = $"/ui/admin/platform/routes/edit?id={info.NavigationParentActionIdentifier}";
            else
                NavigationBackLink.Visible = false;

            ActionUrl.Text = info.ActionUrl;
            ActionUrl1.Text = info.ActionUrl;
            NavigationBackLink.Text = $"<a href='{backUrl}'><i class='fas fa-pencil me-2'></i></a>Action that {info.ActionUrl} navigates back to";

            ActionType.Value = info.ActionType;
            ControllerPath.Text = info.ControllerPath;
            ActionTitle.Text = info.ActionName;
            NavigationText.Text = info.ActionNameShort;
            ActionIcon.Text = info.ActionIcon;

            HelpUrl.Text = info.HelpUrl;

            IsAuthorizationRequirementShiftIqExamEvent.Checked = 
                info.AuthorizationRequirement == ActionAuthorizationRequirement.ShiftIqExamEventAuthentication;
        }

        public void GetInputValues(TAction info)
        {
            info.PermissionParentActionIdentifier = PermissionIdentifier.Value;
            info.ActionList = ActionList.Text;
            info.NavigationParentActionIdentifier = NavigationBack.Value;
            info.ActionUrl = ActionUrl.Text;
            info.ActionType = ActionType.Value;
            info.ControllerPath = ControllerPath.Text;
            info.ActionName = ActionTitle.Text;
            info.ActionNameShort = NavigationText.Text;
            info.ActionIcon = ActionIcon.Text;
            info.HelpUrl = HelpUrl.Text;
            info.AuthorizationRequirement = IsAuthorizationRequirementShiftIqExamEvent.Checked
                ? ActionAuthorizationRequirement.ShiftIqExamEventAuthentication
                : null;
        }
    }
}