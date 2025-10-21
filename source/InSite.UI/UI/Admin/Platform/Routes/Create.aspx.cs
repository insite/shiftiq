using System;
using System.Data.Entity.Infrastructure;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Utilities.Actions.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/platform/routes/search";
        private const string EditUrl = "/ui/admin/platform/routes/edit";

        #region Initialization and loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, "[New Action]");

            CancelButton.NavigateUrl = SearchUrl;
        }

        #endregion

        #region Database operations

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var actionId = Save();
            if (actionId.HasValue)
                HttpResponseHelper.Redirect($"{EditUrl}?id={actionId.Value}&status=saved");
        }

        private Guid? Save()
        {
            try
            {
                var action = new TAction();

                GetInputValues(action);

                TActionStore.Insert(action);

                return action.ActionIdentifier;
            }
            catch (DbUpdateException)
            {
                CreatorStatus.AddMessage(AlertType.Error, "An action already exists for this URL");
            }

            return null;
        }

        #endregion

        #region Getting and setting input values

        public void GetInputValues(TAction info)
        {
            info.PermissionParentActionIdentifier = PermissionSelector.Value.Value;
            info.ActionList = PackageName.Text;
            info.ActionUrl = ActionUrlInput.Text;
            info.ActionName = ActionTitle.Text;
            info.ActionType = "Form";
        }

        #endregion
    }
}
