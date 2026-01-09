using System;

using InSite.UI.Layout.Admin;

namespace InSite.Admin.Assets.Uploads.Forms
{
    public partial class Browse : AdminBasePage
    {
        protected string FileAppUrl => Common.Web.HttpRequestHelper.CurrentRootUrlFiles;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                CompanyName.Value = Organization.CompanyName;
        }
    }
}