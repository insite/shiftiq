using System;
using System.Collections.Generic;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ReorderSurveyQuestionsCommand = InSite.Application.Surveys.Write.ReorderSurveyQuestions;

namespace InSite.Admin.Workflow.Forms.Questions
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request["form"], out var survey) ? survey : Guid.Empty;
        private int? PageNumber => int.TryParse(Request["page"], out var value) ? value : (int?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanCreate)
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search", true);

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
            if (survey == null
                || survey.Form.Tenant != Organization.OrganizationIdentifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search", true);
            }

            PageHelper.AutoBindHeader(this);
            Detail.SetDefaultInputValues(survey, PageNumber);

            CancelButton.NavigateUrl = GetRedirectLink();
        }

        private void Save()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);

            var question = new SurveyQuestion(UniqueIdentifier.Create())
            {
                Form = survey.Form
            };

            survey.Form.Questions.Add(question);

            Detail.GetInputValues(question);

            var commands = new SurveyCommandGenerator().GetCommands(survey.Form.Identifier, question);

            if (question.Sequence != question.Form.Questions.Count)
            {
                var sequences = new Dictionary<Guid, int>();
                foreach (var q in question.Form.Questions)
                    sequences.Add(q.Identifier, q.Sequence);

                commands.Add(new ReorderSurveyQuestionsCommand(SurveyIdentifier, sequences));
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(GetRedirectLink(question.Identifier));
        }

        private string GetRedirectLink(Guid? questionId = null)
        {
            var url = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel=questions";

            if (questionId.HasValue)
                url += $"&question={questionId}";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}&panel=questions"
                : null;
        }
    }
}
