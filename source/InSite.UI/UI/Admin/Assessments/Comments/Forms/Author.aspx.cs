using System;
using System.Web.UI;

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
    public partial class Author : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            SaveAndNewButton.Click += SaveAndNewButton_Click;
            SaveAndAgainButton.Click += SaveAndAgainButton_Click;
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
            var cmd = Save();
            if (cmd != null)
                RedirectToReader();
        }

        private void SaveAndNewButton_Click(object sender, EventArgs e)
        {
            var cmd = Save();
            if (cmd != null)
                HttpResponseHelper.Redirect(GetPostAgainUrl(cmd.Comment, "new"), true);
        }

        private void SaveAndAgainButton_Click(object sender, EventArgs e)
        {
            var cmd = Save();
            if (cmd != null)
                HttpResponseHelper.Redirect(GetPostAgainUrl(cmd.Comment, "again"), true);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            Instructor.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            Instructor.Filter.ParentName = "Training Provider";

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            SetInputValues(bank);
        }

        private PostComment Save()
        {
            if (!Page.IsValid)
                return null;

            var cmd = new PostComment(
                BankID, UniqueIdentifier.Create(),
                MapIndicatorTypeToFlagType(CommentFlag.EnumValue),
                SubjectInputs.SubjectType,
                SubjectInputs.SubjectIdentifier,
                User.UserIdentifier,
                CommentAuthorType.Value,
                CommentCategory.Value,
                CommentText.Text,
                GetInstructorIdentifier(Instructor.Value),
                GetEventDate(EventDate.Text),
                EventFormat.Value,
                DateTimeOffset.UtcNow);

            ServiceLocator.SendCommand(cmd);

            return cmd;
        }

        #endregion

        #region Settings/getting input values

        public static DateTimeOffset? GetEventDate(string text)
        {
            if (DateTime.TryParse(text, out DateTime result))
                return new DateTimeOffset(TimeZoneInfo.ConvertTime(result, User.TimeZone, TimeZones.Utc));
            return null;
        }

        public static Guid? GetInstructorIdentifier(Guid? group)
        {
            return group.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(group.Value)?.GroupIdentifier
                : null;
        }

        private void SetInputValues(BankState bank, Question question = null)
        {
            var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            SubjectInputs.LoadData(bank);

            if (Guid.TryParse(Request["comment"], out var commentId))
            {
                var comment = bank.FindComment(commentId);
                if (comment != null)
                {
                    CommentFlag.EnumValue = MapFlagTypeToIndicatorType(comment.Flag);
                    CommentAuthorType.Value = comment.AuthorRole;
                    EventFormat.Value = comment.EventFormat;

                    if (Request["post"] == "again")
                    {
                        Instructor.Value = comment.Instructor;
                        EventDate.Text = comment.EventDate.HasValue
                            ? TimeZoneInfo.ConvertTime(comment.EventDate.Value, User.TimeZone).ToString("MMM d, yyyy")
                            : null;
                    }

                    if (comment.Type == CommentType.Specification)
                        SubjectInputs.SpecIdentifier = comment.Subject;
                    else if (comment.Type == CommentType.Form)
                        SubjectInputs.FormIdentifier = comment.Subject;
                    else if (Guid.TryParse(Request["form"], out var formValue))
                        SubjectInputs.FormIdentifier = formValue;
                    else if (comment.Type == CommentType.Field)
                        SubjectInputs.QuestionIdentifier = bank.FindField(comment.Subject)?.QuestionIdentifier;
                    else if (comment.Type == CommentType.Question)
                        SubjectInputs.QuestionIdentifier = comment.Subject;
                }
            }
            else
            {
                SubjectInputs.SpecIdentifier = GetGuidNullable(Request["spec"]);
                SubjectInputs.FormIdentifier = GetGuidNullable(Request["form"]);
                SubjectInputs.QuestionIdentifier = GetGuidNullable(Request["question"]);

                Guid? GetGuidNullable(string value) =>
                    Guid.TryParse(value, out var result) ? result : (Guid?)null;
            }

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private FlagType MapIndicatorTypeToFlagType(Indicator? enumValue)
        {
            switch (enumValue)
            {
                case Indicator.Primary:
                    return FlagType.Blue;
                case Indicator.Default:
                    return FlagType.Gray;
                case Indicator.Success:
                    return FlagType.Green;
                case Indicator.Danger:
                    return FlagType.Red;
                case Indicator.Warning:
                    return FlagType.Yellow;
                case Indicator.Info:
                    return FlagType.Cyan;
                case Indicator.Light:
                    return FlagType.White;
                case Indicator.Dark:
                    return FlagType.Black;
            }

            return FlagType.None;
        }

        private Indicator? MapFlagTypeToIndicatorType(FlagType flag)
        {
            switch (flag)
            {
                case FlagType.Blue:
                    return Indicator.Primary;
                case FlagType.Gray:
                    return Indicator.Default;
                case FlagType.Green:
                    return Indicator.Success;
                case FlagType.Red:
                    return Indicator.Danger;
                case FlagType.Yellow:
                    return Indicator.Warning;
                case FlagType.Cyan:
                    return Indicator.Info;
                case FlagType.White:
                    return Indicator.Light;
                case FlagType.Black:
                    return Indicator.Dark;
            }

            return Indicator.None;
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
            return new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}&panel=comments";
        }

        private string GetPostAgainUrl(Guid commentId, string type)
        {
            var url = $"/ui/admin/assessments/comments/author?bank={BankID}&comment={commentId}&post={type}";

            var formId = SubjectInputs.FormIdentifier;
            if (formId.HasValue)
                url += $"&form={formId.Value}";

            return Request.QueryString["return"].IsNotEmpty()
                ? new ReturnUrl(new ReturnUrl().GetReturnUrl()).GetRedirectUrl(url)
                : url;
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