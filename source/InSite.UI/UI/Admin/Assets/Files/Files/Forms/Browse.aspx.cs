using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assets.Files.Files.Forms
{
    public partial class Browse : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Identity.IsInGroup(GroupIdentifiers.PlatformAdministrator))
                HttpResponseHelper.Redirect(RelativeUrl.AdminHomeUrl);

            base.OnLoad(e);

            if (!IsPostBack)
            {
                TreeView.LoadData();
            }
        }
    }
}