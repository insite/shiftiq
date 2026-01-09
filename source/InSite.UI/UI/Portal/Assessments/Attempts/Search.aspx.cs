using System;

using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Portal.Assessments.Attempts
{
    public partial class Search : SearchPage<QAttemptFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Authenticated())
                return;

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        private bool Authenticated()
        {
            if (CurrentSessionState.Identity.IsAuthenticated)
                return true;
            
            HttpResponseHelper.SendHttp403();
            return false;
        }
    }
}