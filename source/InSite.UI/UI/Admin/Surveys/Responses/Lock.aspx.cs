using System;
using System.Collections.Generic;

using Common.Timeline.Commands;

using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Surveys.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Surveys.Responses
{
    public partial class Lock : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid? ResponseIdentifier => Guid.TryParse(Request.QueryString["session"], out var value) ? value : (Guid?)null;

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/surveys/responses/search", true);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockButton.Click += (sender, args) => Continue();
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
                HttpResponseHelper.SendHttp400($"Survey response not found: {ResponseIdentifier}");

            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);
            if (surveyForm == null)
                HttpResponseHelper.SendHttp400($"Survey form not found: {response.SurveyFormIdentifier}");

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

        private void Continue()
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier.Value);
            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);

            LockSurveyInGradebook(response, surveyForm);
            ServiceLocator.SendCommand(new LockResponseSession(ResponseIdentifier.Value));

            HttpResponseHelper.Redirect(GetParentUrl("panel=results"));
        }

        private void LockSurveyInGradebook(QResponseSession response, QSurveyForm surveyForm)
        {
            var commands = SurveyHelper.GetResponseGradebookCommands(response, surveyForm, "Completed");
            if (commands.IsEmpty())
                return;

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, "panel=results");
    }
}
