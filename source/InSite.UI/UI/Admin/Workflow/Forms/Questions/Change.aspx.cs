using System;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Questions
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request["form"], out var survey) ? survey : Guid.Empty;

        private Guid QuestionIdentifier => Guid.TryParse(Request["question"], out var survey) ? survey : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
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

            PageHelper.AutoBindHeader(this);

            Detail.SetInputValues(question);

            CancelButton.NavigateUrl = GetRedirectLink();

            SaveButton.Visible = true;
        }

        private void Save()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            var question = survey != null ? survey.Form.FindQuestion(QuestionIdentifier) : null;

            if (question == null || survey.Form.Tenant != Organization.OrganizationIdentifier)
            {
                Response.Redirect("/ui/admin/workflow/forms/search");
                return;
            }

            var original = question.Clone();
            var originalSequence = question.Sequence;

            Detail.GetInputValues(question);

            var commands = new SurveyCommandGenerator().GetDifferenceCommands(original, question, originalSequence);
            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(GetRedirectLink());
        }

        private string GetRedirectLink()
        {
            return $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&question={QuestionIdentifier}&panel=questions";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}&question={QuestionIdentifier}&panel=questions"
                : null;
        }
    }
}
