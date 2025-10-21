using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Comments.Controls
{
    public partial class WorkshopCommentRepeater : BaseUserControl
    {
        private ReturnUrl _returnUrl;

        public int LoadData(BankState bank, Guid[] subjectIds, ReturnUrl returnUrl, string filterText = null)
        {
            _returnUrl = returnUrl;

            var users = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = bank.Comments.Select(x => x.Author).Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier);

            var commentsQuery = bank.Comments.Where(x => subjectIds.Contains(x.Subject));

            if (!string.IsNullOrEmpty(filterText))
                commentsQuery = commentsQuery.Where(x => x.Text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            var comments = commentsQuery
                .OrderByDescending(x => x.Posted)
                .Select(comment => new
                {
                    BankID = bank.Identifier,
                    CommentID = comment.Identifier,

                    Author = users[comment.Author],
                    PostedOn = comment.Posted,
                    Subject = comment.GetSubjectTitle(bank),
                    Text = Markdown.ToHtml(comment.Text),
                    comment.Category,
                    comment.Flag,
                    FlagIcon = comment.Flag.ToIconHtml(),
                    comment.EventFormat
                }).ToArray();

            Repeater.DataSource = comments;
            Repeater.DataBind();

            return comments.Length;
        }

        protected string GetRedirectUrl(string url, params object[] args) =>
            _returnUrl.GetRedirectUrl(string.Format(url, args));
    }
}