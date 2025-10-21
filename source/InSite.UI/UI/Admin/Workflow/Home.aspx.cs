using System;

using InSite.Application.Issues.Read;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Issues
{
    public partial class Dashboard : AdminBasePage
    {
        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var issues = ServiceLocator.IssueSearch.CountIssues(CreateFilter());

            HistoryPanel.Visible = issues > 0;
            IssueCount.Text = $@"{issues:n0}";
        }

        private QIssueFilter CreateFilter()
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();

            RecentList.LoadData(CreateFilter(), 10);
        }
    }
}