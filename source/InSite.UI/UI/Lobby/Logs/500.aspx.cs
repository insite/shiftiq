using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using InSite.UI.Lobby.Controls;

using Shift.Common;

namespace InSite.UI.Lobby.Logs
{
    public partial class _500 : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ErrorBody.ContentCreated += ErrorBody_ContentCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.StatusCode = 500;

            base.OnLoad(e);
        }

        private void ErrorBody_ContentCreated(object sender, ErrorPageBody.ContentEventArgs e)
        {
            try
            {
                var ex = Server.GetLastError();
                if (ex == null)
                    return;

                var container = e.Container.FindControl("SystemInformation");
                var message = (ITextControl)e.Container.FindControl("ErrorMessage");
                var signOutLink = (HtmlAnchor)e.Container.FindControl("SignOutLink");

                message.Text = ex.ToString();
                signOutLink.HRef = SignOut.GetUrl();
                container.Visible = ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Production;
            }
            catch
            {

            }
        }
    }
}