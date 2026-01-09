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
    public partial class DeleteOne : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid ResponseIdentifier => Guid.TryParse(Request.QueryString["session"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_OnClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier, x => x.SurveyForm, x => x.Respondent);
            if (response == null || response.SurveyForm == null || response.SurveyForm.OrganizationIdentifier != Organization.OrganizationIdentifier)
                Response.Redirect("/ui/admin/workflow/forms/search", true);

            Started.Text = response.ResponseSessionStarted.Format(User.TimeZone, nullValue: "N/A");

            if (response.ResponseSessionCompleted.HasValue)
                Completed.Text = response.ResponseSessionCompleted.Format(User.TimeZone, nullValue: "N/A");

            var survey = ServiceLocator.SurveySearch.GetSurveyState(response.SurveyForm.SurveyFormIdentifier);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Form #{survey.Form.Asset}</span>");

            SurveyLink.HRef = $"/ui/admin/workflow/forms/outline?form={survey.Form.Identifier}";
            InternalName.Text = survey.Form.Name;
            RespondentFields.Visible = !survey.Form.EnableUserConfidentiality;

            var studentUser = ServiceLocator.UserSearch.GetUser(response.RespondentUserIdentifier);
            var studentPerson = ServiceLocator.PersonSearch.GetPerson(response.RespondentUserIdentifier, response.SurveyForm.OrganizationIdentifier);

            PersonDetail.BindPerson(studentPerson, studentUser, User.TimeZone);

            CancelButton.NavigateUrl = GetParentUrl(null);
        }

        private void DeleteButton_OnClick(object sender, EventArgs e)
        {
            var response = ServiceLocator.SurveySearch.GetResponseSession(ResponseIdentifier);

            if (response != null)
            {
                var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(response.SurveyFormIdentifier);
                DeleteSurveyInGradebook(response, surveyForm);

                ServiceLocator.SendCommand(new DeleteResponseSession(ResponseIdentifier));
            }

            RedirectBack();
        }

        private void DeleteSurveyInGradebook(QResponseSession response, QSurveyForm surveyForm)
        {
            var commands = FormHelper.GetResponseGradebookCommands(response, surveyForm, "Delete");
            if (commands.IsEmpty())
                return;

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);
        }

        private void RedirectBack()
        {
            var url = GetParentUrl(null);
            HttpResponseHelper.Redirect(url, true);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/search")
                ? $"panel=results"
                : GetParentLinkParameters(parent, null);
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
        {
            return GetParent();
        }
    }
}