using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Assessments.Attempts.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class PerformanceReport : AdminBasePage
    {
        private VPerformanceReportFilter SearchedFilter
        {
            get => (VPerformanceReportFilter)ViewState[nameof(SearchedFilter)];
            set => ViewState[nameof(SearchedFilter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;
            DownloadScoresButton.Click += DownloadScoresButton_Click;
            DownloadAlternateScoresButton.Click += DownloadAlternateScoresButton_Click;
            DownloadAssessmentAuditAlternateButton.Click += (x, y) => DownloadAssessmentAudit(true);
            DownloadAssessmentAuditButton.Click += (x, y) => DownloadAssessmentAudit(false);

            SelectedCase.AutoPostBack = true;
            SelectedCase.ValueChanged += SelectedCase_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.Toolkits.Assessments?.PerformanceReport?.Enabled != true)
                HttpResponseHelper.Redirect("/");

            PageHelper.AutoBindHeader(this);

            SelectedCase.OrderByOpened = true;

            ReportOptions.BindControls();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            SearchedFilter = ReportCriteria.GetFilter();

            SelectedCase.Filter.TopicUserIdentifier = SearchedFilter.LearnerUserIdentifier;

            var attemptIds = GetScoreDetails(false, true)
                .Select(x => x.Item.AttemptIdentifier)
                .Distinct()
                .ToArray();

            var hasData = attemptIds.Length > 0 && ReportGrid.LoadData(attemptIds);

            NoAttemptPanel.Visible = !hasData;
            AttemptPanel.Visible = hasData;
        }

        private void DownloadScoresButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DownloadScores("item-scores", false);
        }

        private void DownloadAlternateScoresButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DownloadScores("item-scores-alternate", true);
        }

        private void DownloadScores(string fileName, bool useAlternateFramework)
        {
            SearchedFilter.AttemptIds = ReportGrid.GetSelectedAttempts();

            var config = ReportOptions.GetReportConfig();
            var csv = PerformanceReportHelper.GetScoresCsv(config, SearchedFilter, useAlternateFramework);
            var csvBytes = Encoding.UTF8.GetBytes(csv);
            var reportType = config.RequiredRole.ToString().ToLower();
            var reportUser = ServiceLocator.PersonSearch.GetPerson(SearchedFilter.LearnerUserIdentifier, Organization.Identifier, x => x.User);

            Response.SendFile(GenerateFileName(fileName, reportType, reportUser), csvBytes, "text/csv");
        }

        string GenerateFileName(string fileName, string reportType, QPerson reportUser)
        {
            var components = new List<string>
            {
                fileName,
                reportType,
                reportUser?.User?.FullName?.Replace(" ", "-").ToLower(),
                reportUser?.PersonCode
            }.Where(component => !string.IsNullOrWhiteSpace(component));

            return $"{string.Join("-", components)}.csv";
        }

        private void SelectedCase_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            SearchedFilter.AttemptIds = ReportGrid.GetSelectedAttempts();

            var issueId = SelectedCase.Value ?? throw new ArgumentNullException("selectedCase");
            var issue = ServiceLocator.IssueSearch.GetIssue(issueId);
            var useAlternateFramework = ReportType.Value == "Alternate";

            PerformanceReportHelper.SaveReportToCase(ReportOptions.GetReportConfig(), SearchedFilter, useAlternateFramework, issueId);

            var caseLink = $"<a target=_blank href='/ui/admin/workflow/cases/outline?case={issueId}&panel=attachments'>Case #{issue.IssueNumber}</a>";

            FileUploadedAlert.AddMessage(AlertType.Success, $"The report was successfully generated and saved to the {caseLink}");

            SelectedCase.Value = null;
        }

        private void DownloadAssessmentAudit(bool isAlternate)
        {
            if (!IsValid)
                return;

            var reportIndex = isAlternate ? 0 : (int?)null;
            var scoresAndSequence = GetScoreDetails(isAlternate, false, reportIndex);

            if (scoresAndSequence.Count == 0)
            {
                ErrorAlert.AddMessage(AlertType.Error, "There are no scores to build the report");
                return;
            }

            var scores = scoresAndSequence.Select(x => x.Score).ToList();
            var userId = SearchedFilter.LearnerUserIdentifier;

            var person = PersonSearch.Select(Organization.Identifier, userId, x => x.User)
                ?? throw new ArgumentException($"User {userId} is not found in the organization {Organization.Identifier}");

            var xlsx = AssessmentAuditReport.GetXlsx(userId, person.PersonCode, scores, isAlternate);
            Response.SendFile("assessment-audit.xlsx", xlsx, "application/vnd.ms-excel");
        }

        private List<PerformanceReportHelper.ScoreDetail> GetScoreDetails(bool useAlternateFramework, bool allAttempts = false, int? reportIndex = null)
        {
            SearchedFilter.AttemptIds = !allAttempts ? ReportGrid.GetSelectedAttempts() : null;

            var reportConfig = ReportOptions.GetReportConfig(reportIndex);

            return PerformanceReportHelper.GetScoreDetails(reportConfig, SearchedFilter, useAlternateFramework);
        }
    }
}