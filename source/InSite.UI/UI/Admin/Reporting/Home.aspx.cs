using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Reporting
{
    public partial class Home : AdminBasePage
    {
        public int tempQueryCount;
        public bool tempQueryResult;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            tempQueryResult = int.TryParse($"{InSite.Admin.Reports.Queries.Controls.QuerySearchCriteria.GetReports().Length}", out tempQueryCount);
            tempQueryCount = (tempQueryResult == true) ? tempQueryCount : 0;

            ImpersonationsCounter.Visible = Identity.IsOperator;

            QueriesLabel.InnerText = Organization.CompanyName + " Queries";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            try
            {
                DatabaseQueryRow.Visible = Identity.IsOperator;

                CurrentSessionGrid.LoadData();
            }
            catch (Exception)
            {
            }

            PageHelper.AutoBindHeader(this);

            var isE03 = ServiceLocator.Partition.IsE03();

            var isE03Admin = Identity.IsInRole(CmdsRole.SystemAdministrators);

            FrequentlyUsedReports.Visible = !isE03 || isE03Admin;

            CurrentUserSessions.Visible = !isE03 || isE03Admin;

            var isQueriesVisible = tempQueryCount > 0 || Identity.IsOperator;
            LoadCounter(QueryCounter, QueryCount, isQueriesVisible, tempQueryCount, QueryLink, "/ui/admin/reports/queries/search");

            var filter = new TUserSessionFilter();
            if (!Identity.IsOperator)
                filter.OrganizationIdentifier = Organization.OrganizationIdentifier;

            var authenticationCount = TUserSessionSearch.Count(filter);
            LoadCounter(AuthenticationsCounter, AuthenticationsCount, Identity.IsOperator, authenticationCount, AuthenticationsLink, "/ui/admin/reporting/login-history");

            var impersonationsCount = ImpersonationSearch.Count(new ImpersonationFilter());
            LoadCounter(ImpersonationsCounter, ImpersonationsCount, Identity.IsOperator, impersonationsCount, ImpersonationsLink, "/ui/admin/reports/impersonations/search");

            SystemAdministrationPanel.Visible = Identity.IsGranted(PermissionNames.Custom_CMDS_Administrators);
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}