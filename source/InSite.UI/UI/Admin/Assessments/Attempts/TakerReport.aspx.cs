using System;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Assessments.Attempts.Controls;
using InSite.UI.Admin.Assessments.Attempts.Utilities.TakerReport;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class TakerReport : AdminBasePage
    {
        private QAttemptFilter SearchedFilter
        {
            get => (QAttemptFilter)ViewState[nameof(SearchedFilter)];
            set => ViewState[nameof(SearchedFilter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;

            SelectedCase.AutoPostBack = true;
            SelectedCase.ValueChanged += SelectedCase_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            SelectedCase.OrderByOpened = true;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            SearchedFilter = ReportCriteria.GetFilter();

            SelectedCase.Filter.TopicUserIdentifier = SearchedFilter.LearnerUserIdentifier;

            var hasData = ReportGrid.LoadData(SearchedFilter);

            NoAttemptPanel.Visible = !hasData;
            AttemptPanel.Visible = hasData;
        }

        private void SelectedCase_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var issueId = SelectedCase.Value ?? throw new ArgumentNullException("selectedCase");
            var issue = ServiceLocator.IssueSearch.GetIssue(issueId);

            SaveReport(issueId);

            var caseLink = $"<a target=_blank href='/ui/admin/workflow/cases/outline?case={issueId}&panel=attachments'>Case #{issue.IssueNumber}</a>";

            FileUploadedAlert.AddMessage(AlertType.Success, $"The report was successfully generated and saved to the {caseLink}");

            SelectedCase.Value = null;
        }

        private void SaveReport(Guid issueId)
        {
            var userId = SearchedFilter.LearnerUserIdentifier ?? throw new ArgumentNullException("SearchedFilter.LearnerUserIdentifier");
            var organizationId = SearchedFilter.FormOrganizationIdentifier ?? throw new ArgumentNullException("SearchedFilter.FormOrganizationIdentifier");
            var attemptIds = ReportGrid.GetSelectedAttempts();

            var report = TakerReportControl.GetPdf(this, userId, attemptIds, ReportOptions.Language);

            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone);
            var language = ReportOptions.Language == TakerReportControl.Language.English ? "en" : "fr";
            var documentName = $"{date:yyyyMMdd} High Stakes Test Taker Report ({language})";

            TakerReportUploader.SaveReportToCase(report, issueId, documentName);
        }
    }
}
