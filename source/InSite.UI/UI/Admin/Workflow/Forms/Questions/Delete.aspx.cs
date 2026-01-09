using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using IHasParentLinkParameters = Shift.Common.IHasParentLinkParameters;
using IWebRoute = Shift.Common.IWebRoute;
using RemoveSurveyQuestionCommand = InSite.Application.Surveys.Write.DeleteSurveyQuestion;

namespace InSite.Admin.Workflow.Forms.Questions
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        private Guid QuestionIdentifier => Guid.TryParse(Request.QueryString["question"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            var question = survey != null ? survey.Form.FindQuestion(QuestionIdentifier) : null;

            if (question == null
                || survey.Form.Tenant != Organization.OrganizationIdentifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search", true);
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Form #{survey.Form.Asset}</span>");

            SurveyLink.HRef = $"/ui/admin/workflow/forms/outline?form={survey.Form.Identifier}";
            InternalName.Text = survey.Form.Name;

            if (question.Page != null)
                QuestionPage.Text = question.Page.Sequence.ToString();
            QuestionNumber.Text = question.Sequence.ToString();
            QuestionText.Text = Markdown.ToHtml(question.Content?.Title?.Text.Default ?? "(Question)");
            QuestionType.Text = question.Type.GetDescription();

            var optionItemCount = question.Options.Lists.Sum(x => x.Items.Count);
            var answerCount = ServiceLocator.SurveySearch.CountResponseAnswers(QuestionIdentifier);

            OptionListCount.Text = question.Options.Lists.Count > 0 ? $"{question.Options.Lists.Count}" : "0";
            OptionItemCount.Text = optionItemCount > 0 ? $"{optionItemCount}" : "0";
            AnswerCount.Text = answerCount > 0 ? $"{answerCount}" : "0";

            CancelButton.NavigateUrl = GetOutlineUrl(QuestionIdentifier);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);

            ServiceLocator.SendCommand(new RemoveSurveyQuestionCommand(survey.Form.Identifier, QuestionIdentifier));

            RedirectToOutline(null);
        }

        #region Methods (navigation)

        private void RedirectToOutline(Guid? questionId)
        {
            var url = GetOutlineUrl(questionId);
            HttpResponseHelper.Redirect(url, true);
        }

        private string GetOutlineUrl(Guid? questionId)
        {
            var query = string.Empty;

            if (questionId.HasValue)
                query += $"&question={questionId.Value}";

            return $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel=questions" + query;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}&question={QuestionIdentifier}&panel=questions"
                : null;
        }

        #endregion
    }
}