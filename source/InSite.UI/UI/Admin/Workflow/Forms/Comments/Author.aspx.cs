using System;
using System.Web.UI;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Comments
{
    public partial class Author : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyId => Guid.TryParse(Request["form"], out Guid value) ? value : Guid.Empty;

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
            var survey = ServiceLocator.SurveySearch.GetSurveyForm(SurveyId);
            if (survey == null || survey.OrganizationIdentifier != Organization.OrganizationIdentifier)
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, null, survey.SurveyFormName);

            CommentUpload.FolderPath = $"/Forms/{SurveyId}/CommentAttachments";

            CancelButton.NavigateUrl = GetParentUrl();
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new PostSurveyComment(
                SurveyId, UniqueIdentifier.Create(),
                CommentText.Value, CommentFlag.FlagValue, CommentResolved.Value));

            RedirectToParent();
        }

        #region Navigation

        public const string NavigateUrl = "/ui/admin/workflow/forms/comments/author";

        public static string GetNavigateUrl(Guid surveyFormId) => NavigateUrl + "?form=" + surveyFormId;

        public static void Redirect(Guid surveyFormId) => HttpResponseHelper.Redirect(GetNavigateUrl(surveyFormId));

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