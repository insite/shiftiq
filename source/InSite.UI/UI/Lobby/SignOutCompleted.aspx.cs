using System;
using System.Web.UI;

using InSite.Common.Web;

namespace InSite.UI.Lobby
{
    public partial class SignOutCompleted : Page
    {
        private string ReturnUrl => Request.QueryString["returnurl"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var urls = ServiceLocator.Urls;

            var url = urls.LoginUrl;

            if (!string.IsNullOrEmpty(ReturnUrl))
                url += "?returnurl=" + ReturnUrl;

            HttpResponseHelper.Redirect(url);
        }
    }
}