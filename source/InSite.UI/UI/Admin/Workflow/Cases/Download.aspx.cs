using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Workflow.Cases.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Issues.Issues.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CaseIdentifier => Guid.TryParse(Request.QueryString["case"], out var value) ? value : (Guid?)null;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = CaseIdentifier.HasValue ? ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value) : null;
            if (issue == null || !CaseVisibilityHelper.IsCaseVisible(issue.OrganizationIdentifier, issue.TopicUserIdentifier))
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/cases/search");
                return;
            }

            PageHelper.AutoBindHeader(this, qualifier: $"{issue.IssueTitle} <span class='fw-normal fs-md text-body-secondary'>Case #{issue.IssueNumber} - {issue.IssueType}</span>");

            CaseInfo.BindIssue(issue, User.TimeZone);

            SetupDownloadSection();

            CancelLink.NavigateUrl = $"/ui/admin/workflow/cases/outline?case={CaseIdentifier}";
        }

        private void SetupDownloadSection()
        {
            FileName.Text = string.Format("issue-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;
            if (fileFormat == "JSON")
                SendJson();
        }

        private void SendJson()
        {
            var data = CaseHelper.Serialize(ServiceLocator.IssueSearch.GetIssue(CaseIdentifier.Value));
            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"case={CaseIdentifier}"
                : null;
        }
    }
}