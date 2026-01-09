using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Web.Helpers;

namespace InSite.UI.Layout.Lobby
{
    public partial class LobbyLogin : MasterPage
    {
        protected static ISecurityFramework Identity => CurrentSessionState.Identity;
        protected static Domain.Organizations.OrganizationState Organization => Identity.Organization;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var isIe = HttpRequestHelper.IsIE;
            HeaderCurrentScripts.Visible = !isIe;
            FooterCurrentScripts.Visible = !isIe;

            var wallpaperUrl = WallpaperManager.GetCoverUrl(Organization?.PlatformCustomization?.PlatformUrl?.Wallpaper, Organization.Code);

            Wallpaper.Attributes.Add("style", $"z-index:-999; top: 0; right: 0; background-image: url({wallpaperUrl});");

            var logoUrl = WallpaperManager.GetLogoUrl(Organization?.PlatformCustomization?.PlatformUrl?.Logo, Organization.Code);

            SignInLogo.Src = logoUrl;
            SignInIcon.Src = logoUrl;

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
    }
}