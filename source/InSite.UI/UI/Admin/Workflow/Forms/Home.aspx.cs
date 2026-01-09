using System;

using InSite.Application.Surveys.Read;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Home : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindModelToControls();

                LoadSurveys();
                LoadSessions();

                RecentChanges.LoadData(5);
                HistoryPanel.Visible = RecentChanges.ItemCount > 0;
            }
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);
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