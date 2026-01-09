using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Profile.Employee.Certificate
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        private const string EditUrl = "/ui/cmds/admin/validations/college-certificates/edit";
        private const string SearchUrl = "/ui/cmds/admin/validations/college-certificates/search";

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

            PageHelper.AutoBindHeader(this);

            CloseButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var info = new TCollegeCertificate();

            Details.GetInputValues(info);

            TCollegeCertificateStore.Insert(info);

            var url = $"{EditUrl}?id={info.LearnerIdentifier}&certificateID={info.ProfileIdentifier}&status=saved";

            HttpResponseHelper.Redirect(url, true);
        }
    }
}