using System;

using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Surveys.Forms.Forms
{
    public partial class DeleteSurveyForm : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier => Guid.TryParse(Request["survey"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteResponsesCheckBox.AutoPostBack = true;
            DeleteResponsesCheckBox.CheckedChanged += DeleteResponsesCheckBox_CheckedChanged;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void DeleteResponsesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeleteButton.Enabled = DeleteResponsesCheckBox.Checked;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value) : null;

            if (survey == null
                || survey.Form.Tenant != CurrentSessionState.Identity.Organization.Identifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/surveys/search", true);
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Survey Form #{survey.Form.Asset}</span>");

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            var messageCount = survey.Form.Messages.Count;
            var courseCount = ServiceLocator.PageSearch.Count(x => x.ObjectType == "Survey" && x.ObjectIdentifier == survey.Form.Identifier);
            var respondentCount = survey.Form.Respondents.Count;
            var responseCount = ServiceLocator.SurveySearch.CountResponseSessions(new QResponseSessionFilter { SurveyFormIdentifier = survey.Form.Identifier });
            var questionCount = survey.Form.Questions.Count;

            MessageCount.Text = $"{messageCount:n0}";
            CourseCount.Text = $"{courseCount:n0}";
            ResponseCount.Text = $"{responseCount:n0}";
            QuestionCount.Text = $"{questionCount:n0}";

            var allowVoid = true;
            var hasResponses = responseCount > 0;

            DeleteWarning.Visible = DeleteButton.Visible = allowVoid;

            DeleteButton.Enabled = !hasResponses;
            DeleteResponsesPanel.Visible = allowVoid && hasResponses;

            ConfirmLiteral.Text = "Are you sure you want to delete this survey?";

            CancelButton.NavigateUrl = $"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value);

            if (survey != null)
            {
                var filter = new QResponseSessionFilter { SurveyFormIdentifier = survey.Form.Identifier };
                var responses = ServiceLocator.SurveySearch.GetResponseSessions(filter);
                foreach (var response in responses)
                    ServiceLocator.SendCommand(new Application.Responses.Write.DeleteResponseSession(response.ResponseSessionIdentifier));

                var respondents = survey.Form.Respondents;
                ServiceLocator.SendCommand(new Application.Surveys.Write.DeleteSurveyRespondents(survey.Form.Identifier, respondents.ToArray()));
                ServiceLocator.SendCommand(new Application.Surveys.Write.DeleteSurveyForm(SurveyIdentifier.Value));

                Course2Store.ClearCache(Organization.Identifier);
            }

            HttpResponseHelper.Redirect("/ui/admin/surveys/search");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"survey={SurveyIdentifier}"
                : null;
        }
    }
}