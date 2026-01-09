using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Integrations
{
    public partial class Home : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            APIRequestCounter.Visible = Identity.IsOperator;

            var insite = CurrentSessionState.Identity.IsOperator;

            var apiRequestCounts = ApiRequestSearch.Count(new ApiRequestFilter());
            LoadCounter(APIRequestCounter, APIRequestCount, insite, apiRequestCounts, APIRequestLink, "/ui/admin/integrations/api-requests/search");
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}