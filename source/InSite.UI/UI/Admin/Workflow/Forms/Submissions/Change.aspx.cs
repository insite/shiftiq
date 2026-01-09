using System;

using InSite.Application.Responses.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Submissions
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ResponseIdentifier => Guid.TryParse(Request.QueryString["submission"], out var value) ? value : (Guid?)null;

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/submissions/search", true);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;

            SaveButton.Click += (sender, args) => Continue();
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
            {
                HttpResponseHelper.SendHttp400($"Form submission not found: {ResponseIdentifier}");
                return;
            }

            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);
            var userTimeZone = User.TimeZone.Id;
            var studentPerson = ServiceLocator.PersonSearch.GetPerson(response.RespondentUserIdentifier, surveyForm.OrganizationIdentifier, x => x.User);

            if (!surveyForm.EnableUserConfidentiality && studentPerson != null)
                userTimeZone = studentPerson.User.TimeZone;

            PageHelper.AutoBindHeader(this, null, surveyForm.SurveyFormName);

            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyForm.SurveyFormIdentifier);
            SurveyDetail.BindSurvey(survey, User.TimeZone);

            PersonDetail.BindPerson(studentPerson, User.TimeZone);

            ResponseStatus.Text = response.ResponseSessionStatus;
            ResponseStarted.Text = response.ResponseSessionStarted.Format(userTimeZone, "-");
            ResponseCompleted.Text = response.ResponseSessionCompleted.Format(userTimeZone, "-");

            CancelButton.NavigateUrl = GetOutlineUrl(ResponseIdentifier.Value);

            GroupIdentifier.Value = response.GroupIdentifier;
            PeriodIdentifier.Value = response.PeriodIdentifier;

            RespondentFields.Visible = !surveyForm.EnableUserConfidentiality;

        }

        private void Continue()
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier.Value);

            if (response.GroupIdentifier != GroupIdentifier.Value)
                ServiceLocator.SendCommand(new ChangeResponseGroup(ResponseIdentifier.Value, GroupIdentifier.Value));

            if (response.PeriodIdentifier != PeriodIdentifier.Value)
                ServiceLocator.SendCommand(new ChangeResponsePeriod(ResponseIdentifier.Value, PeriodIdentifier.Value));

            RedirectToOutline();
        }

        private void RedirectToOutline() =>
            HttpResponseHelper.Redirect(GetOutlineUrl(ResponseIdentifier.Value), true);

        private string GetOutlineUrl(Guid response) =>
            $"/ui/admin/workflow/forms/submissions/outline?session={response}";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => $"session={ResponseIdentifier}";
    }
}
