using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Web.Helpers;

using Shift.Sdk.UI;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminBase : MasterPage
    {
        protected ISecurityFramework Identity => CurrentSessionState.Identity;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ThemeStyleSheet.Url = StyleHelper.Get();
        }

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
            HtmlBody.Attributes["class"] = $"{Request.Browser.Platform} {Request.Browser.Browser}{Request.Browser.MajorVersion}";
        }
    }
}