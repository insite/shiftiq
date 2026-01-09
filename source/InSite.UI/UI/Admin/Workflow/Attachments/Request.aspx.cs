using System;

using InSite.Application.Cases.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Issues.Attachments
{
    public partial class Require : AdminBasePage, IHasParentLinkParameters
    {
        private Guid CaseIdentifier => Guid.TryParse(Request.QueryString["case"], out var value) ? value : Guid.Empty;

        private string DefaultFileCategory => Request.QueryString["category"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileCategory.AutoPostBack = true;
            FileCategory.ValueChanged += FileCategory_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var issue = ServiceLocator.IssueSearch.GetIssue(CaseIdentifier);
            if (issue == null || issue.OrganizationIdentifier != Organization.Identifier)
                RedirectToSearch();

            CaseInfo.BindIssue(issue, User.TimeZone, true, false);

            LoadData();

            PageHelper.AutoBindHeader(this, null,
                $"{issue.IssueTitle} <span class='form-text'>{issue.IssueType} Case #{issue.IssueNumber}</span>");

            CancelButton.NavigateUrl = GetOutlineUrl();
        }

        private void FileCategory_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            FileSubcategory.ClearSelection();
            FileSubcategory.DocumentType = FileCategory.Value;
            FileSubcategory.RefreshData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();

            RedirectToOutline();
        }

        private void Save()
        {
            var existing = ServiceLocator.IssueSearch.GetFileRequirement(CaseIdentifier, FileCategory.Value);
            if (existing != null
                && string.Equals(existing.RequestedFileSubcategory, FileSubcategory.Value, StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.RequestedFrom, RequestedFrom.Value, StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.RequestedFileDescription.EmptyIfNull(), FileDescription.Text.EmptyIfNull(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.RequestedFileStatus.EmptyIfNull(), FileStatus.Value.EmptyIfNull(), StringComparison.OrdinalIgnoreCase)
                )
            {
                return;
            }

            ServiceLocator.SendCommand(new ModifyFileRequirement(
                CaseIdentifier,
                FileCategory.Value,
                FileSubcategory.Value,
                RequestedFrom.Value,
                FileDescription.Text,
                FileStatus.Value
            ));
        }

        private void LoadData()
        {
            if (string.IsNullOrEmpty(DefaultFileCategory))
                return;

            FileCategory.EnsureDataBound();
            FileCategory.Value = DefaultFileCategory;

            FileCategory_ValueChanged(null, null);

            var existing = ServiceLocator.IssueSearch.GetFileRequirement(CaseIdentifier, DefaultFileCategory);
            if (existing == null)
                return;

            FileCategory.Enabled = false;

            FileSubcategory.Value = existing.RequestedFileSubcategory;
            RequestedFrom.Value = existing.RequestedFrom;
            FileDescription.Text = existing.RequestedFileDescription;

            FileStatus.EnsureDataBound();
            FileStatus.Value = existing.RequestedFileStatus;

            SaveButton.Text = "Save";
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