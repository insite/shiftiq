using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Web.Helpers;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminBase : MasterPage
    {
        protected ISecurityFramework Identity => CurrentSessionState.Identity;

        protected override void OnLoad(EventArgs e)
        {
            HttpResponseHelper.SetNoCache();

            if (IsPostBack)
                AntiForgeryHelper.Validate();

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var className = $"{Request.Browser.Platform} {Request.Browser.Browser}{Request.Browser.MajorVersion}";
            if (ModeSwitch.GetCurrentThemeMode() == "Dark")
                className += " dark-theme";

            HtmlBody.Attributes["class"] = className;
        }
    }
}