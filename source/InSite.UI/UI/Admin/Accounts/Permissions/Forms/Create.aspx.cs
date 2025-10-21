using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Permissions.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/permissions/edit";
        private const string SearchUrl = "/ui/admin/accounts/permissions/search";

        private string ReturnUrl
        {
            get => ViewState[nameof(ReturnUrl)] as string;
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            ReturnUrl = GetReturnUrl();

            var toolkitId = ValueConverter.ToGuidNullable(Request.QueryString["toolkit"]);
            var groupId = ValueConverter.ToGuidNullable(Request.QueryString["group"]);

            Detail.SetDefaultInputValues(toolkitId, groupId);

            CancelButton.NavigateUrl = ReturnUrl ?? SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Detail.Save(out var permissionId);

            var returnUrl = ReturnUrl
                ?? $"{EditUrl}?id={permissionId}&status=saved";

            HttpResponseHelper.Redirect(returnUrl);
        }
    }
}