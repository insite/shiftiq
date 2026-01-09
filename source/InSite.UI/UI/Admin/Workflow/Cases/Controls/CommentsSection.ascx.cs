using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Issues.Outlines.Utilities;
using InSite.Application.Contents.Read;
using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Issues.Outlines.Controls
{
    public partial class CommentsSection : BaseUserControl
    {
        #region Properties

        public int Count => Repeater.Items.Count;

        protected Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        public Func<VIssue> LoadIssue { get; internal set; }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadCommentsXlsx.Click += DownloadCommentsXlsx_Click;
            SearchInput.Click += SearchInput_Click;
        }

        #endregion

        #region Event handlers

        private void DownloadCommentsXlsx_Click(object sender, EventArgs e)
        {
            var issue = LoadIssue();
            var downloader = new CaseDownloader(User.TimeZone);
            DownloadCommentsXlsx.Visible = downloader.DownloadCommentsToExcel(issue, Page.Response) == 0;
        }

        private void SearchInput_Click(object sender, EventArgs e)
        {
            SetInputValues();
        }

        #endregion

        #region Data binding

        public void LoadData(Guid issue)
        {
            IssueIdentifier = issue;

            SetInputValues();

            AddComment.NavigateUrl = $"/ui/admin/workflow/comments/create?case={IssueIdentifier}";
        }

        #endregion

        #region Getting and setting input values

        private void SetInputValues()
        {
            Repeater.DataSource = null;

            var comments = GetComments();

            Repeater.DataSource = comments.OrderByDescending(x => x.CommentPosted);
            Repeater.DataBind();

            CommentPanel.Visible = comments.Count > 0;

            if (!IsPostBack)
            {
                SearchInput.Visible = comments.Count > 0;

                DownloadCommentsXlsx.Visible = comments.Count > 0;
            }
        }

        private List<VComment> GetComments()
        {
            var filter = new QIssueCommentFilter()
            {
                OrganizationIdentifier = Organization.Identifier,
                IssueIdentifier = IssueIdentifier
            };

            var comments = ServiceLocator.IssueSearch.GetComments(filter);
            var hasData = comments.Any();

            if (hasData && !string.IsNullOrEmpty(SearchInput.Text))
                comments = comments.Where(x => IsCommentMatch(x, SearchInput.Text)).ToList();

            return comments;
        }

        #endregion

        #region Helper methods

        private static bool IsCommentMatch(VComment comment, string keyword)
        {
            return comment.CommentText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected string GetTimestamp(object item)
        {
            var comment = (VComment)item;
            var author = comment.AuthorUserName;
            var time = LocalizeTime(comment.CommentPosted, null, false);
            return $"<span title='{time}'>posted by {author} {time}</span>";
        }

        protected string GetCommentBody(object item)
        {
            var comment = (VComment)item;
            if (comment.CommentText != null)
                return Markdown.ToHtml(comment.CommentText);
            return string.Empty;
        }

        protected string GetCommentSubject(object item)
        {
            var comment = (VComment)item;

            var body = comment.CommentText;

            if (string.IsNullOrEmpty(body))
                return string.Empty;

            body = StringHelper.Snip(body, 100);

            int index = body.IndexOf(Environment.NewLine);
            if (index >= 0)
                return body.Substring(0, index);

            return body;
        }

        #endregion
    }
}