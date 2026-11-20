using System;
using System.Text;

using Humanizer;

using InSite.Application.Issues.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Issues.Issues.Controls
{
    public partial class CaseInfo : System.Web.UI.UserControl
    {
        public void BindIssue(VIssue issue, bool showTitle = true, bool showType = true, bool showDesc = true)
        {
            IssueLink.HRef = $"/ui/admin/workflow/cases/outline?case={issue.IssueIdentifier}";
            IssueNumber.InnerText = $"Case #{issue.IssueNumber}";
            IssueStatus.Text = $"{issue.IssueStatusName} {issue.IssueStatusCategoryHtml}";

            IssueType.Text = issue.IssueType ?? "None";
            IssueTitle.Text = issue.IssueTitle;
            IssueDescription.Text = Markdown.ToHtml(issue.IssueDescription ?? "None");

            TitleDiv.Visible = showTitle;
            TypeDiv.Visible = showType;
            DescriptionDiv.Visible = showDesc;
        }
    }
}
