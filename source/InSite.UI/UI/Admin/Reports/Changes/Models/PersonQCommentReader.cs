using System;
using System.Web;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    internal static class PersonQCommentReader
    {
        public static void Read(Guid userId, Guid organizationId, HistoryCollection history)
        {
            var comments = PersonCommentSummarySearch.SelectForCommentRepeater(userId, organizationId);

            foreach (var comment in comments)
            {
                var who = comment.AuthorUserName.IfNullOrEmpty(UserNames.Someone);
                var what = $"- [Comment](/ui/admin/contacts/comments/revise?contact={comment.ContainerIdentifier}&comment={comment.CommentIdentifier}) added: "
                    + HttpUtility.HtmlEncode(comment.CommentText.MaxLength(60, true));

                history.Add(comment.CommentPosted.UtcDateTime, who, what);
            }
        }
    }
}