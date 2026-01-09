using System;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Submissions
{
    public partial class Unlock : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid? ResponseIdentifier => Guid.TryParse(Request.QueryString["session"], out var value) ? value : (Guid?)null;

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/submissions/search", true);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnlockButton.Click += (sender, args) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!ResponseIdentifier.HasValue)
                RedirectToSearch();

            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier.Value);
            if (response == null)
                HttpResponseHelper.SendHttp400($"Form submission not found: {ResponseIdentifier}");

            if (!response.ResponseSessionCompleted.HasValue)
                RedirectToSearch();

            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);
            if (surveyForm == null)
                HttpResponseHelper.SendHttp400($"Form not found: {response.SurveyFormIdentifier}");

            var userTimeZone = User.TimeZone.Id;
            var studentUser = ServiceLocator.UserSearch.GetUser(response.RespondentUserIdentifier);
            var studentPerson = ServiceLocator.PersonSearch.GetPerson(response.RespondentUserIdentifier, surveyForm.OrganizationIdentifier);

            if (!surveyForm.EnableUserConfidentiality && studentUser != null)
                userTimeZone = studentUser.TimeZone;

            PageHelper.AutoBindHeader(this, null, surveyForm.SurveyFormName);

            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyForm.SurveyFormIdentifier);
            SurveyDetail.BindSurvey(survey, User.TimeZone);

            PersonDetail.BindPerson(studentPerson, studentUser, User.TimeZone);

            ResponseStatus.Text = response.ResponseSessionStatus;
            ResponseStarted.Text = response.ResponseSessionStarted.Format(userTimeZone, "-");
            ResponseCompleted.Text = response.ResponseSessionCompleted.Format(userTimeZone, "-");
            RespondentFields.Visible = !surveyForm.EnableUserConfidentiality;

            CancelButton.NavigateUrl = GetParentUrl("panel=results");
        }

        private void UnlockSurveyInGradebook(QResponseSession response, QSurveyForm surveyForm)
        {
            var commands = FormHelper.GetResponseGradebookCommands(response, surveyForm, "Started");
            if (commands.IsEmpty())
                return;

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        private void Continue()
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier.Value);
            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);

            UnlockSurveyInGradebook(response, surveyForm);
            ServiceLocator.SendCommand(new UnlockResponseSession(ResponseIdentifier.Value));

            HttpResponseHelper.Redirect(GetParentUrl("panel=results"));
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, "panel=results");
    }
}
