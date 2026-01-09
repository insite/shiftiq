using System;

using InSite.Application.Issues.Read;
using InSite.Application.Surveys.Read;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Issues
{
    public partial class Dashboard : AdminBasePage
    {
        protected void BindCases()
        {
            var issues = ServiceLocator.IssueSearch.CountIssues(CreateFilter());

            HistoryPanel.Visible = issues > 0;
            IssueCount.Text = $@"{issues:n0}";
            CaseStatusesLink.Visible = Identity.IsOperator;

            RecentCaseChanges.LoadData(CreateFilter(), 10);
        }

        private void BindForms()
        {
            LoadSurveys();
            LoadSessions();

            RecentFormChanges.LoadData(5);

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

            PageHelper.AutoBindHeader(this);

            BindCases();

            BindForms();

            HistoryPanel.Visible = RecentCaseChanges.ItemCount + RecentFormChanges.ItemCount > 0;
        }

        private void LoadSurveys()
        {
            var filter = new QSurveyFormFilter()
            {
                OrganizationIdentifier = Organization.Identifier
            };

            var count = ServiceLocator.SurveySearch.CountSurveyForms(filter);
            SurveyFormCount.Text = $@"{count:n0}";
        }

        private void LoadSessions()
        {
            var filter = new QResponseSessionFilter()
            {
                OrganizationIdentifier = Organization.Identifier
            };
            var count = ServiceLocator.SurveySearch.CountResponseSessions(filter);
            ResponseSessionCount.Text = $@"{count:n0}";
        }
    }
}