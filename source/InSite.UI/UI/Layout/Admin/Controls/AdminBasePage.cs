using System;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Admin
{
    public abstract class AdminBasePage : Portal.PortalBasePage
    {
        protected enum StatusType { Saved, Translated }

        protected virtual bool CanCreate
        {
            get => ViewState[nameof(CanCreate)] == null || (bool)ViewState[nameof(CanCreate)];
            set => ViewState[nameof(CanCreate)] = value;
        }

        protected virtual bool CanEdit
        {
            get => ViewState[nameof(CanEdit)] == null || (bool)ViewState[nameof(CanEdit)];
            set => ViewState[nameof(CanEdit)] = value;
        }

        protected virtual bool CanDelete
        {
            get => ViewState[nameof(CanDelete)] == null || (bool)ViewState[nameof(CanDelete)];
            set => ViewState[nameof(CanDelete)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Page.Master is AdminHome a)
                a.RenderHelpContent(ActionModel);

            if (Identity.IsAuthenticated)
                TToolkitVisitStore.Visit(Organization.Identifier, User.Identifier, CookieTokenModule.Current.ID, ActionModel.ActionIdentifier);
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            CanCreate = CurrentSessionState.Identity.IsGranted(Route.ToolkitNumber, DataAccess.Update);
            CanEdit = CurrentSessionState.Identity.IsGranted(Route.ToolkitNumber, DataAccess.Update);
            CanDelete = CurrentSessionState.Identity.IsGranted(Route.ToolkitNumber, DataAccess.Delete);
        }

        protected void SetStatus(Alert alertControl, StatusType status)
        {
            switch (status)
            {
                case StatusType.Saved:
                    alertControl.AddMessage(AlertType.Success, "Your changes are now saved.");
                    break;
                case StatusType.Translated:
                    alertControl.AddMessage(AlertType.Success, "Your content is now translated.");
                    break;
                default:
                    throw new ArgumentException($"Unknown status: {status}");
            }
        }
    }
}
