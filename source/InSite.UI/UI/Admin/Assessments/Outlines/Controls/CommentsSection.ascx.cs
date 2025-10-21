using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Outlines.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class CommentsSection : BaseUserControl
    {
        #region Properties

        public Func<BankState> LoadBank { get; internal set; }

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadCommentsXlsx.Click += DownloadCommentsXlsx_Click;

            SearchInput.Click += FilterCommentsButton_Click;

            Repeater.ItemCommand += Repeater_ItemCommand;
        }

        #endregion

        #region Event handlers

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "JumpToQuestion")
            {
                var questionId = Guid.Parse(e.CommandArgument.ToString());
                var question = ServiceLocator.BankSearch.GetQuestion(questionId);

                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={question.BankIdentifier}&question={questionId}");
            }
        }

        private void DownloadCommentsXlsx_Click(object sender, EventArgs e)
        {
            var bank = LoadBank();
            var downloader = new BankDownloader(User.TimeZone);
            DownloadCommentsXlsx.Visible = downloader.DownloadCommentsToExcel(bank, Page.Response) == 0;
        }

        private void FilterCommentsButton_Click(object sender, EventArgs e)
        {
            var bank = LoadBank();

            LoadData(bank, CanWrite, out var _);
        }

        #endregion

        #region Data binding

        public void LoadData(BankState bank, bool canWrite, out bool isSelected)
        {
            CanWrite = canWrite;

            _returnUrl = new ReturnUrl("bank&panel=comments");

            AddCommentLink.NavigateUrl = _returnUrl.GetRedirectUrl($"/ui/admin/assessments/comments/author?bank={bank.Identifier}");
            AddCommentLink.Visible = canWrite;
            DownloadCommentsXlsx.Visible = bank.Comments.Count > 0;

            isSelected = false;
            if (!IsPostBack && Request.QueryString["panel"] == "comments")
                isSelected = true;

            var filterText = SearchInput.Text;
            var users = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = bank.Comments.Select(x => x.Author).Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier);

            Repeater.DataSource = bank.Comments
                .Where(x => string.IsNullOrEmpty(filterText) || !string.IsNullOrEmpty(x.Text) && x.Text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(x => x.Posted)
                .Select(comment =>
                {
                    var postedOn = TimeZoneInfo.ConvertTime(comment.Posted, User.TimeZone);
                    Guid questionID = Guid.Empty;
                    bool isQuestion = false;

                    if (comment.Type == CommentType.Field)
                    {
                        var field = (bank.FindField(comment.Subject));
                        if (field != null)
                        {
                            questionID = field.QuestionIdentifier;
                            isQuestion = true;
                        }
                    }
                    else if (comment.Type == CommentType.Question)
                    {
                        var question = (bank.FindQuestion(comment.Subject));
                        if (question != null)
                        {
                            questionID = question.Identifier;
                            isQuestion = true;
                        }
                    }

                    return new
                    {
                        BankID = bank.Identifier,
                        CommentID = comment.Identifier,
                        QuestionID = questionID,
                        QuestionVisible = isQuestion,
                        AuthorName = users.TryGetValue(comment.Author, out var user)
                            ? user.FullName
                            : comment.Author.ToString(),
                        PostedOn = GetCommentPostedDateTime(comment),
                        Subject = comment.GetSubjectTitle(bank),
                        Text = Markdown.ToHtml(comment.Text),
                        comment.Category,
                        comment.Flag,
                        FlagIcon = comment.Flag.ToIconHtml(),
                        comment.EventFormat
                    };
                });
            Repeater.DataBind();

            SearchInput.Visible = bank.Comments.Count > 0;
            SearchInput.Visible = bank.Comments.Count > 0;

            string GetCommentPostedDateTime(Comment comment)
            {
                return $"<span title='{LocalizeTime(comment.Posted, null, false)}'>commented " + TimeZones.Format(comment.Posted, CurrentSessionState.Identity.User.TimeZone) + "</span>";
            }
        }

        protected string GetRedirectUrl(string url, params object[] args) =>
            _returnUrl.GetRedirectUrl(string.Format(url, args));

        #endregion
    }
}