using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities
{
    public partial class Search : SearchPage<TOpportunityFilter>
    {
        private TOpportunityFilter Filter
        {
            get => ViewState[nameof(Filter)] as TOpportunityFilter;
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

            Layout.Admin.PageHelper.AutoBindHeader(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
    }
}