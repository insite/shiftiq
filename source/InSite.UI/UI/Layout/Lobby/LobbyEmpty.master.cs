using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Web.Helpers;

namespace InSite.UI.Layout.Lobby
{
    public partial class LobbyEmpty : MasterPage
    {
        protected override void OnLoad(EventArgs e)
        {
            HttpResponseHelper.SetNoCache();

            if (IsPostBack)
                AntiForgeryHelper.Validate();

            base.OnLoad(e);
        }
    }
}