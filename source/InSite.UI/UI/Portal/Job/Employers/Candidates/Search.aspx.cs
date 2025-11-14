using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Jobs.Employers.Candidates
{
    public partial class Search : SearchPage<JobPersonFilter>
    {
        private JobPersonFilter Filter
        {
            get => ViewState[nameof(Filter)] as JobPersonFilter;
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this, null, "Candidates");
            PortalMaster.SidebarVisible(false);
        }
    }
}