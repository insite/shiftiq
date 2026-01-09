using System;
using System.Text;
using System.Web.UI;

using Humanizer;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Report : SearchPage<QResponseSessionFilter>, IHasParentLinkParameters
    {
        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? "form=" + SurveyID
                : null;
        }

        #endregion

        #region Classes

        private class SurveyInfo
        {
            public int ID { get; internal set; }
            public string Name { get; internal set; }
            public int CompletedCount { get; internal set; }
            public int TotalCount { get; internal set; }
        }

        #endregion

        #region Properties

        private bool HasResults
        {
            get => (bool?)ViewState[nameof(HasResults)] == true;
            set => ViewState[nameof(HasResults)] = value;
        }

        private Guid? SurveyID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchCriteria.Searching += SearchCriteria_Searching;

            DistributionCriteria.Searching += DistributionCriteria_Searching;
            DistributionCriteria.Clearing += DistributionCriteria_Clearing;

            CorrelationCriteria.Searching += CorrelationCriteria_Searching;
            CorrelationCriteria.Clearing += CorrelationCriteria_Clearing;
            CorrelationCriteria.Alert += Control_Alert;

            DownloadSection.Alert += Control_Alert;

            PrintDistributionReport.Click += PrintDistributionReport_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SearchCriteria.Clear();

            if (!SurveyID.HasValue || !LoadSurvey(SurveyID.Value))
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search", true);

            DistributionCriteria.Clear();
            CorrelationCriteria.Clear();
        }

        #endregion

        #region Data binding

        private bool LoadSurvey(Guid id)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(id);
            if (survey == null)
                return false;
            else
                return LoadSurvey(survey.Form, SearchCriteria.Filter);
        }

        private bool ReloadSurvey(QResponseSessionFilter responseFilter = null)
        {
            var survey = DistributionCriteria.SurveyID.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyState(DistributionCriteria.SurveyID.Value)
                : null;

            return LoadSurvey(survey?.Form, responseFilter);
        }

        private bool LoadSurvey(SurveyForm survey, QResponseSessionFilter responseFilter = null)
        {
            if (survey == null || survey.Tenant != Organization.Identifier)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Form or submissions not found.");
                return false;
            }

            var responseCount = responseFilter == null ?
                ServiceLocator.SurveySearch.CountResponseSessions(
                    new QResponseSessionFilter { SurveyFormIdentifier = survey.Identifier }) :
                ServiceLocator.SurveySearch.CountResponseSessions(responseFilter);

            var subtitle = "submission".ToQuantity(responseCount, "N0");
            var title = $"{survey.Name} <span class=\"form-text\">{subtitle}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            HasResults = responseCount > 0;
            if (HasResults)
            {
                var responses = responseFilter == null
                    ? ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter { SurveyFormIdentifier = survey.Identifier })
                    : ServiceLocator.SurveySearch.GetResponseSessions(responseFilter);

                DistributionCriteria.SurveyID = survey.Identifier;
                CorrelationCriteria.SurveyID = survey.Identifier;
                DownloadSection.SurveyID = survey.Identifier;

                DateAnalysisChart.LoadData(survey, responses);
                SurveyInvitationAnalytics.Bind(survey.Identifier);
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Information, "There are no completed submissions for this form.");
                NavPanel.Visible = false;
            }

            PrintDistributionReport.Visible = DistributionCriteria.SurveyID.HasValue && HasResults;

            return true;
        }

        #endregion

        #region Event handlers

        private void PrintDistributionReport_Click(object sender, EventArgs e)
        {
            var url = GetReportPrintUrl();
            if (string.IsNullOrEmpty(url))
                return;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Report),
                "redirect",
                $"window.open('{url}');", true);
        }

        protected string GetReportPrintUrl()
        {
            var filter = DistributionResults.Filter;
            if (filter == null || !DistributionCriteria.SurveyID.HasValue)
                return null;

            var filterJson = JsonConvert.SerializeObject(filter);
            var filterBytes = Encoding.UTF8.GetBytes(filterJson);
            var filterBase64 = Convert.ToBase64String(filterBytes);

            return $"/ui/admin/workflow/forms/report-print?form={SurveyID}&filter={filterBase64}";
        }

        private void Control_Alert(object sender, AlertArgs args) => ScreenStatus.AddMessage(args);

        protected override void LoadSearchedResults()
        {
            base.LoadSearchedResults();

            ReloadSurvey(SearchCriteria.Filter);
        }

        private void SearchCriteria_Searching(object sender, EventArgs e) => OnSearchCriteriaSearch();

        private void OnSearchCriteriaSearch()
        {
            ReloadSurvey(SearchCriteria.Filter);
        }

        private void DistributionCriteria_Searching(object sender, EventArgs e) => OnDistributionSearch();

        private void OnDistributionSearch()
        {
            DistributionResults.Search(DistributionCriteria.Filter, SearchCriteria.Filter);

            OnCorrelationClear();

            DistributionResultsPanel.Visible = true;
        }

        private void DistributionCriteria_Clearing(object sender, EventArgs e) => OnDistributionClear();

        private void OnDistributionClear()
        {
            DistributionResults.Clear(DistributionCriteria.Filter);

            DistributionResultsPanel.Visible = false;
        }

        private void DeleteResultsButton_Click(object sender, EventArgs e)
        {
            if (DistributionCriteria.SurveyID.HasValue)
            {
                var sessions = ServiceLocator.SurveySearch.GetResponseSessions(new QResponseSessionFilter
                {
                    SurveyFormIdentifier = DistributionCriteria.SurveyID.Value
                });

                foreach (var session in sessions)
                    ServiceLocator.SendCommand(new DeleteResponseSession(session.ResponseSessionIdentifier));

                ScreenStatus.AddMessage(
                    AlertType.Success,
                    $"All of the results for this form have been deleted: {"record".ToQuantity(sessions.Count)}");
            }

            ReloadSurvey();
        }

        private void CorrelationCriteria_Searching(object sender, EventArgs e) => OnCorrelationSearch();

        private void OnCorrelationSearch()
        {
            CorrelationResults.Search(CorrelationCriteria.Filter);

            OnDistributionClear();

            CorrelationResultsPanel.Visible = true;
        }

        private void CorrelationCriteria_Clearing(object sender, EventArgs e) => OnCorrelationClear();

        private void OnCorrelationClear()
        {
            CorrelationResults.Clear(CorrelationCriteria.Filter);

            CorrelationResultsPanel.Visible = false;
        }

        #endregion
    }
}