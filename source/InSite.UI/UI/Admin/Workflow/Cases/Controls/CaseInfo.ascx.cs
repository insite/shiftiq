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
        public void BindIssue(VIssue issue, TimeZoneInfo tz, bool showTitle = true, bool showType = true, bool showDesc = true)
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

        private static string GetIssueTimestamp(Guid id, TimeZoneInfo tz)
        {
            var issue = ServiceLocator.IssueSearch.GetIssue(id);
            var when = issue.IssueOpened;
            var who = UserSearch.GetFullName(issue.IssueOpenedBy);

            var sb = new StringBuilder();

            if (issue.IssueStatusCategory == "Closed" && issue.IssueClosed.HasValue)
            {
                when = issue.IssueClosed.Value;
                who = UserSearch.GetFullName(issue.IssueClosedBy);
                sb.AppendFormat("<div>{0} closed this issue {1} ({2})</div>", who, when.Humanize(), when.Format(tz, true));
            }

            sb.AppendFormat("{0} opened this issue {1} ({2})", who, when.Humanize(), when.Format(tz, true));

            return sb.ToString();
        }
    }
}