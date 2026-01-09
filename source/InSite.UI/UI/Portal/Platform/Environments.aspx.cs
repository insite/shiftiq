using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Settings.Environments
{
    public partial class Select : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!CurrentSessionState.Identity.IsAdministrator)
            {
                EnvironmentPanel.Visible = false;
                return;
            }

            PageHelper.AutoBindHeader(this);

            var url = Page.Request.Url;
            var organization = UrlHelper.GetOrganizationCode(url);
            var domain = ServiceLocator.AppSettings.Security.Domain;

            GoToDevelopment.NavigateUrl = $"https://dev-{organization}.{domain}";
            GoToSandbox.NavigateUrl = $"https://sandbox-{organization}.{domain}";
            GoToProduction.NavigateUrl = $"https://{organization}.{domain}";

            if (ServiceLocator.Partition.IsE03())
            {
                GoToDevelopment.Text = "Test";
                GoToSandbox.Text = "Demo";
            }

            PageHelper.HideSideContent(Page);
        }
    }
}