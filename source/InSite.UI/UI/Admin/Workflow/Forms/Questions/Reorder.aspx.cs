using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ReorderSurveyQuestionsCommand = InSite.Application.Surveys.Write.ReorderSurveyQuestions;

namespace InSite.Admin.Workflow.Forms.Questions
{
    public partial class Reorder : AdminBasePage, IHasParentLinkParameters
    {
        protected Guid SurveyIdentifier => Guid.TryParse(Page.Request.QueryString["form"], out var id) ? id : Guid.Empty;

        private Guid[] Questions
        {
            get => (Guid[])ViewState[nameof(Questions)];
            set => ViewState[nameof(Questions)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                Open();

            base.OnLoad(e);
        }

        public override void ApplyAccessControl()
        {
            if (!CanEdit) SaveButton.Visible = false;
            base.ApplyAccessControl();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var args = Request.Form["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(args))
                return;

            var isSave = args.StartsWith("save&");

            if (isSave)
            {
                var startIndex = args.IndexOf('&') + 1;

                Save(args.Substring(startIndex));
            }

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel=questions");
        }

        private void Open()
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

            PageHelper.AutoBindHeader(this, null, "Reorder the questions in a form");

            Questions = survey.Form.Questions.Select(x => x.Identifier).ToArray();

            QuestionRepeater.DataSource = survey.Form.Questions.Select(x =>
                {
                    var code = !string.IsNullOrEmpty(x.Code)
                        ? x.Code
                        : x.Sequence.ToString();
                    var style = x.GetIndicatorStyleName();

                    return new
                    {
                        Code = $"<span class=\"badge bg-{style}\">{code}</span>",
                        TitleHtml = x.Content?.Title?.GetHtml()
                    };
                });

            QuestionRepeater.DataBind();

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel=questions";
        }

        private void Save(string args)
        {
            var sequences = new Dictionary<Guid, int>();
            var oldSequences = args.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < oldSequences.Length; i++)
            {
                var oldSequence = int.Parse(oldSequences[i]);
                var question = Questions[oldSequence - 1];

                sequences.Add(question, i + 1);
            }

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);

            var command = new ReorderSurveyQuestionsCommand(survey.Form.Identifier, sequences);

            ServiceLocator.SendCommand(command);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}&panel=questions"
                : null;
        }
    }
}
