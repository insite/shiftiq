using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Issues.Read;
using InSite.Common.Web;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Admin.Issues.Outlines.Utilities
{
    public class CaseDownloader
    {
        private readonly TimeZoneInfo _tz;

        public CaseDownloader(TimeZoneInfo tz)
        {
            _tz = tz;
        }

        public int DownloadCommentsToExcel(VIssue issue, HttpResponse response)
        {
            var comments = GetComments(issue);
            if (comments.Count == 0)
                return 0;

            var bytes = GetExportData(comments);
            var filename = string.Format("{0} Comments {1:yyyy-MM-dd}",
                StringHelper.Sanitize(issue.IssueTitle, '_'), DateTime.Now);

            response.SendFile(filename, "xlsx", bytes);

            return comments.Count;
        }

        private List<CaseExportCommentItem> GetComments(VIssue issue)
        {
            var filter = new QIssueCommentFilter()
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                IssueIdentifier = issue.IssueIdentifier
            };

            var comments = ServiceLocator.IssueSearch.GetComments(filter);

            var items = comments
                .OrderByDescending(x => x.CommentPosted)
                .Select(x => new CaseExportCommentItem
                {
                    CommentAuthored = x.CommentPosted.Format(_tz),
                    CommentRevised = x.CommentRevised.Format(_tz, nullValue: string.Empty),
                    CommentText = x.CommentText,

                    AuthorName = x.AuthorUserName,
                    RevisorName = x.RevisorUserName
                })
                .ToList();

            return items;
        }

        private byte[] GetExportData(List<CaseExportCommentItem> items)
        {
            var helper = new XlsxExportHelper();

            helper.Map("CommentText", "Comment", 100, HorizontalAlignment.Left);

            helper.Map("AuthorName", "Author", 20, HorizontalAlignment.Left);
            helper.Map("RevisorName", "Revisor", 20, HorizontalAlignment.Left);

            helper.Map("CommentAuthored", "Authored", 26, HorizontalAlignment.Left);
            helper.Map("CommentRevised", "Revised", 26, HorizontalAlignment.Left);

            return helper.GetXlsxBytes(items, "Comments");
        }
    }
}