using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class CommentRepeater : BaseUserControl
    {
        #region Properties

        public bool AllowEdit
        {
            get => ViewState[nameof(AllowEdit)] == null || (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        public bool AllowHide
        {
            get => ViewState[nameof(AllowHide)] == null || (bool)ViewState[nameof(AllowHide)];
            set => ViewState[nameof(AllowHide)] = value;
        }

        public bool ShowAuthor
        {
            get => ViewState[nameof(ShowAuthor)] == null || (bool)ViewState[nameof(ShowAuthor)];
            set => ViewState[nameof(ShowAuthor)] = value;
        }

        #endregion

        #region Properties

        private ReturnUrl _returnUrl;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(CommentRepeater).FullName;
        }

        #endregion

        #region Methods (data binding)

        internal void LoadData(Guid bankId, IEnumerable<Comment> comments, ReturnUrl returnUrl)
        {
            _returnUrl = returnUrl;

            Dictionary<Guid, string> authors;

            if (ShowAuthor)
            {
                authors = UserSearch
                    .Bind(
                        x => new { x.UserIdentifier, x.FullName },
                        new UserFilter { IncludeUserIdentifiers = comments.Select(x => x.Author).Distinct().ToArray() })
                    .ToDictionary(x => x.UserIdentifier, x => x.FullName);
            }
            else
            {
                authors = new Dictionary<Guid, string>();
            }

            Repeater.DataSource = comments.OrderBy(x => x.Posted).Select(x => new
            {
                BankID = bankId,
                CommentID = x.Identifier,

                Author = authors.TryGetValue(x.Author, out var author) ? author : x.Author.ToString(),
                Posted = "posted " + x.Posted.Humanize(),
                IconHtml = x.Flag.ToIconHtml(),
                Text = x.Text,
                IsHidden = x.IsHidden
            });
            Repeater.DataBind();
        }

        internal void LoadData(Guid bankId, Guid questionId, IEnumerable<Comment> comments, ReturnUrl returnUrl)
        {
            LoadData(bankId, comments, returnUrl);

            AddCommentLink.NavigateUrl = _returnUrl.GetRedirectUrl($"/ui/admin/assessments/comments/author?bank={bankId}&question={questionId}");
            AddCommentLink.Visible = true;
        }

        protected string GetRedirectUrl(string url, params object[] args) =>
            _returnUrl.GetRedirectUrl(string.Format(url, args));

        #endregion
    }
}