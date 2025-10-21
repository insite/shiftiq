using System;
using System.Web;

using InSite.UI.Layout.Lobby.Controls;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInFailed : SignInBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ErrorViewCloseButton.Click += ErrorViewCloseButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var message = HttpContext.Current.Request.QueryString["message"];
            if (string.IsNullOrEmpty(message))
                RedirectToParent();

            if (!bool.TryParse(HttpContext.Current.Request.QueryString["htmlEncode"], out bool encode))
                encode = false;

            if (!encode)
                ErrorViewMessage.InnerHtml = HttpUtility.HtmlDecode(message);
            else
                ErrorViewMessage.InnerText = message;
        }

        private void ErrorViewCloseButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }
    }
}