using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assets.Files.Files.Forms
{
    public partial class Browse : AdminBasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            PageHelper.AutoBindHeader(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            var isPlatformAdmin = Identity.IsGranted(PermissionNames.Admin_Settings);

            if (!isPlatformAdmin)
                HttpResponseHelper.Redirect(RelativeUrl.AdminHomeUrl);

            TreeView.LoadData();
        }
    }
}