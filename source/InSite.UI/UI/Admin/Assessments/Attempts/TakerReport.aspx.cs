using System;
using System.IO;

using InSite.Application.Attempts.Read;
using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Assessments.Attempts.Controls;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class TakerReport : AdminBasePage
    {
        private const string FileExtension = ".pdf";

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

            var report = TakerReportControl.GetPdf(this, userId, organizationId, attemptIds, ReportOptions.Language);

            SaveReportToCase(report, issueId);
        }

        private void SaveReportToCase(byte[] report, Guid issueId)
        {
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;
            var userId = CurrentSessionState.Identity.User.Identifier;
            var props = CreateFileProps(issueId);
            var fileName = CreateFileName(issueId, props.DocumentName);

            FileStorageModel model;

            using (var file = new MemoryStream(report))
            {
                model = ServiceLocator.StorageService.Create(
                    file,
                    fileName,
                    organizationId,
                    userId,
                    issueId,
                    FileObjectType.Issue,
                    props,
                    null
                );
            }

            var command = new AddAttachment(
                issueId,
                model.FileName,
                Path.GetExtension(model.FileName),
                model.FileIdentifier,
                DateTimeOffset.UtcNow,
                userId
            );

            ServiceLocator.SendCommand(command);
        }

        private static string CreateFileName(Guid issueId, string documentName)
        {
            var namePart = ServiceLocator.StorageService.AdjustFileName(documentName.Substring(0, documentName.Length - FileExtension.Length));
            var fileName = namePart + FileExtension;
            var number = 1;

            while (ServiceLocator.IssueSearch.GetAttachment(issueId, fileName) != null)
            {
                fileName = namePart + "-" + number + FileExtension;
                number++;
            }

            return fileName;
        }

        private FileProperties CreateFileProps(Guid issueId)
        {
            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone);
            var language = ReportOptions.Language == TakerReportControl.Language.English ? "en" : "fr";
            var documentName = $"{date:yyyyMMdd} High Stakes Test Taker Report ({language}){FileExtension}";

            return new FileProperties
            {
                DocumentName = documentName,
                Status = "System Generated",
                AllowLearnerToView = CaseAttachmentHelper.AllowLearnerToViewByIssue(issueId)
            };
        }
    }
}