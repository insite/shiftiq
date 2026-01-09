using System;
using System.Web.UI;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Comments
{
    public partial class Revise : AdminBasePage, IHasParentLinkParameters
    {
        private Guid CommentId => Guid.Parse(Request["comment"]);

        private Guid? SurveyId
        {
            get => (Guid?)ViewState[nameof(SurveyId)];
            set => ViewState[nameof(SurveyId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (x, y) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var comment = QCommentSearch.SelectFirst(CommentId);
            if (comment == null || comment.OrganizationIdentifier != Organization.Identifier || comment.ContainerType != SurveyStore.CommentContainerType)
                RedirectToSearch();

            var survey = ServiceLocator.SurveySearch.GetSurveyForm(comment.ContainerIdentifier);
            if (survey == null || survey.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToSearch();

            SurveyId = survey.SurveyFormIdentifier;

            PageHelper.AutoBindHeader(this, null, survey.SurveyFormName);

            CommentText.Value = comment.CommentText;
            CommentUpload.FolderPath = $"/Surveys/{comment.ContainerIdentifier}/CommentAttachments";

            CommentFlag.FlagValue = comment.CommentFlag.ToEnumNullable<FlagType>();
            CommentResolved.Value = comment.CommentResolved;

            RemoveButton.NavigateUrl = Delete.GetNavigateUrl(comment.CommentIdentifier);
            CancelButton.NavigateUrl = GetParentUrl();
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ModifySurveyComment(
                SurveyId.Value, CommentId,
                CommentText.Value, CommentFlag.FlagValue, CommentResolved.Value));

            RedirectToParent();
        }

        #region Navigation

        public const string NavigateUrl = "/ui/admin/workflow/forms/comments/revise";

        public static string GetNavigateUrl(Guid commentId) => NavigateUrl + "?comment=" + commentId;

        public static void Redirect(Guid commentId) => HttpResponseHelper.Redirect(GetNavigateUrl(commentId));

        private static void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search", true);

        private void RedirectToParent()
        {
            var url = GetParentUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetParentUrl()
        {
            return $"/ui/admin/workflow/forms/outline?form={SurveyId}&panel=notes";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyId}"
                : null;
        }

        #endregion

    }
}