using System;
using System.Web;
using System.Web.UI;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Issues.Attachments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid CaseIdentifier => Guid.TryParse(Request.QueryString["case"], out var value) ? value : Guid.Empty;

        private string FileName => Request.QueryString["file"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);
            if (issue == null || issue.OrganizationIdentifier != Organization.Identifier)
                RedirectToSearch();

            var (attachment, file) = GetAttachment();

            if (attachment == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, null, $"Case #{issue.IssueNumber}");

            var fileNameEncoded = HttpUtility.HtmlEncode(attachment.FileName);
            var downloadUrl = file != null ? ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName, true) : null;

            FileNameOutput.Text = !string.IsNullOrEmpty(downloadUrl)
                ? $"<a href='{downloadUrl}'>{fileNameEncoded}</a>"
                : $"{fileNameEncoded} <label class='badge bg-danger'>File Not Found</label>";

            PostedOnOutput.Text = attachment.FileUploaded.Format(User.TimeZone);
            PostedByOutput.Text = attachment.InputterUserName;
            IssueNumber.Text = $"<a href=\"/ui/admin/workflow/cases/outline?case={issue.IssueIdentifier}\">{issue.IssueNumber}</a>";
            IssueTitle.Text = $"<a href=\"/ui/admin/workflow/cases/outline?case={issue.IssueIdentifier}\">{issue.IssueTitle}</a>";

            CancelButton.NavigateUrl = GetOutlineUrl();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var (attachment, file) = GetAttachment();

            if (attachment != null)
            {
                var command = new DeleteAttachment(CaseIdentifier, attachment.FileName);
                ServiceLocator.SendCommand(command);
            }

            if (file != null)
                ServiceLocator.StorageService.Delete(file.FileIdentifier);

            RedirectToOutline();
        }

        private (VIssueAttachment, FileStorageModel) GetAttachment()
        {
            var attachment = ServiceLocator.IssueSearch.GetAttachment(CaseIdentifier, FileName);

            var (status, file) = attachment != null && attachment.FileIdentifier != Guid.Empty
                ? ServiceLocator.StorageService.GetFileAndAuthorize(Identity, attachment.FileIdentifier)
                : (FileGrantStatus.NoFile, null);

            return status != FileGrantStatus.Denied ? (attachment, file) : (null, null);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"case={CaseIdentifier}"
                : null;
        }

        private string GetOutlineUrl() =>
            $"/ui/admin/workflow/cases/outline?case={CaseIdentifier}&panel=attachments";

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(), true);

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect("/ui/admin/workflow/cases/search", true);
    }
}