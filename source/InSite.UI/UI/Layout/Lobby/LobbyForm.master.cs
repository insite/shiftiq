using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Web.Helpers;

namespace InSite.UI.Layout.Lobby
{
    public partial class LobbyForm : MasterPage
    {
        protected static ISecurityFramework Identity => CurrentSessionState.Identity;
        protected static Domain.Organizations.OrganizationState Organization => Identity.Organization;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var isIe = HttpRequestHelper.IsIE;
            HeaderCurrentScripts.Visible = !isIe;
            FooterCurrentScripts.Visible = !isIe;

            var logoUrl = string.Empty;
            if (Organization != null)
            {
                if (!string.IsNullOrWhiteSpace(Organization.PlatformCustomization.PlatformUrl.Logo))
                    logoUrl = Organization.PlatformCustomization.PlatformUrl.Logo;
                else if (ServiceLocator.Partition.IsE03())
                    logoUrl = "/library/images/logos/cmds.png";
            }
            if (!string.IsNullOrEmpty(logoUrl))
            {
                SignInLogo.Src = logoUrl;
                SignInIcon.Src = logoUrl;
            }

            SignInLogo.Alt = Organization.Name;
            SignInIcon.Alt = Organization.Name;
        }

        protected override void OnLoad(EventArgs e)
        {
            BodyForm.Action = Request.RawUrl;

            HttpResponseHelper.SetNoCache();

            if (IsPostBack)
                AntiForgeryHelper.Validate();

            base.OnLoad(e);
        }

        public void SetMainWidth(int columns)
        {
            if (IsPostBack)
                return;

            if (columns == 12)
            {
                SideContent.Visible = false;
                SideColumn.Attributes["class"] = "d-none";
                MainColumn.Attributes["class"] = "col-lg-12";
            }
            else
            {
                if (columns < 1 || columns > 12)
                    columns = 9;

                SideContent.Visible = true;
                SideColumn.Attributes["class"] = $"col-lg-{12 - columns}";
                MainColumn.Attributes["class"] = $"col-lg-{columns}";
            }
        }
    }
}