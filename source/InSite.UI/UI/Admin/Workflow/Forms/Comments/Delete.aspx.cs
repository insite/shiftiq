using System;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Comments
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid CommentId => Guid.Parse(Request["comment"]);

        private Guid? SurveyId
        {
            get => (Guid?)ViewState[nameof(SurveyId)];
            set => ViewState[nameof(SurveyId)] = value;
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RemoveButton.Click += (x, y) => Remove();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Database operations

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

            CommentText.Text = Markdown.ToHtml(comment.CommentText);
            CommentAuthor.Text = UserSearch.GetFullName(comment.AuthorUserIdentifier);
            CommentPosted.Text = comment.CommentPosted.Format(User.TimeZone);

            SurveyName.Text = survey.SurveyFormName;

            CancelButton.NavigateUrl = GetParentUrl();
        }

        private void Remove()
        {
            ServiceLocator.SendCommand(new DeleteSurveyComment(SurveyId.Value, CommentId));

            RedirectToParent();
        }

        #endregion

        #region Navigation

        public const string NavigateUrl = "/ui/admin/workflow/forms/comments/delete";

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
                : parent.Name.EndsWith("/revise")
                    ? $"comment={CommentId}"
                    : null;
        }

        #endregion
    }
}