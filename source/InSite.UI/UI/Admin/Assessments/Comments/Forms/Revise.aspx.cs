using System;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Comments.Forms
{
    public partial class Revise : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid CommentID => Guid.TryParse(Request.QueryString["comment"], out var value) ? value : Guid.Empty;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            Instructor.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            Instructor.Filter.AncestorName = "Training Provider";

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var comment = bank.FindComment(CommentID);
            if (comment == null || bank.Tenant != Organization.OrganizationIdentifier)
                RedirectToReader();

            SetInputValues(bank, comment);
        }

        private ReviseComment Save()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                return null;

            var comment = bank.FindComment(CommentID);
            if (comment == null)
                return null;

            if (comment.AuthorRole != CommentAuthorType.Value)
                ServiceLocator.SendCommand(new ChangeCommentAuthorRole(
                    BankID,
                    CommentID,
                    CommentAuthorType.Value));

            if (SubjectInput.Visible && comment.Subject != SubjectInput.SubjectIdentifier)
                ServiceLocator.SendCommand(new MoveComment(
                    BankID,
                    CommentID,
                    SubjectInput.SubjectType,
                    SubjectInput.SubjectIdentifier));

            var cmd = new ReviseComment(
                BankID,
                CommentID,
                User.UserIdentifier,
                CommentFlag.EnumValue ?? FlagType.None,
                FeedbackCategory.Value,
                CommentText.Text,
                Author.GetInstructorIdentifier(Instructor.Value),
                Author.GetEventDate(EventDate.Text),
                EventFormat.Value,
                DateTimeOffset.UtcNow);

            ServiceLocator.SendCommand(cmd);

            return cmd;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(BankState bank, Comment comment)
        {
            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            var allowEdit = comment.Type == CommentType.Bank
                || comment.Type == CommentType.Specification
                || comment.Type == CommentType.Form
                || comment.Type == CommentType.Field
                || comment.Type == CommentType.Question;

            SubjectInput.Visible = allowEdit;
            SubjectOutput.Visible = !allowEdit;

            if (allowEdit)
            {
                SubjectInput.LoadData(bank);

                if (comment.Type == CommentType.Specification)
                {
                    SubjectInput.SpecIdentifier = comment.Subject;
                }
                else if (comment.Type == CommentType.Form)
                {
                    var form = bank.FindForm(comment.Subject);
                    SubjectInput.SpecIdentifier = form.Specification.Identifier;
                    SubjectInput.FormIdentifier = form.Identifier;
                }
                else if (comment.Type == CommentType.Field)
                {
                    var field = bank.FindField(comment.Subject);
                    SubjectInput.SpecIdentifier = field.Section.Form.Specification.Identifier;
                    SubjectInput.FormIdentifier = field.Section.Form.Identifier;
                    SubjectInput.QuestionIdentifier = field.Identifier;
                }
                else if (comment.Type == CommentType.Question)
                {
                    SubjectInput.QuestionIdentifier = comment.Subject;
                }
            }
            else
            {
                SubjectOutput.LoadData(bank, comment);
            }

            CommentFlag.EnsureDataBound();
            CommentFlag.EnumValue = comment.Flag;

            CommentAuthorType.Value = comment.AuthorRole;

            FeedbackCategory.Value = comment.Category;
            CommentText.Text = comment.Text;

            SetInstructorIdentifier(comment.Instructor);
            SetEventDate(comment.EventDate);
            EventFormat.Value = comment.EventFormat;

            RemoveButton.NavigateUrl = new ReturnUrl("bank&comment&return")
                .GetRedirectUrl($"/admin/assessments/comments/delete?bank={BankID}&comment={CommentID}");
            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private void SetEventDate(DateTimeOffset? date)
        {
            if (date.HasValue)
                EventDate.Text = date.Value.FormatDateOnly(User.TimeZone);
        }

        private void SetInstructorIdentifier(Guid? id)
        {
            if (!id.HasValue)
                return;

            var g = ServiceLocator.GroupSearch.GetGroup(id.Value);
            if (g != null)
                Instructor.Value = g.GroupIdentifier;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl()
        {
            var url = new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}&panel=comments";

            var scroll = Request.QueryString["scroll"];
            if (scroll.IsNotEmpty())
                url = HttpResponseHelper.BuildUrl(url, "scroll=" + scroll);

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}