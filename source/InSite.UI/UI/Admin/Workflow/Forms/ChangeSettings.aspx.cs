using System;
using System.Text;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ChangeSurveyDisplaySummaryChart = InSite.Application.Surveys.Write.ChangeSurveyDisplaySummaryChart;
using ChangeSurveyFormSettingsCommand = InSite.Application.Surveys.Write.ChangeSurveyFormSettings;

namespace InSite.Admin.Workflow.Forms
{
    public partial class ChangeSettings : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request["form"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }
        public override void ApplyAccessControl()
        {
            if (!CanEdit) SaveButton.Visible = false;
            base.ApplyAccessControl();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            var surveyForm = survey?.Form;
            if (surveyForm == null
                || surveyForm.Tenant != Organization.Identifier
                || surveyForm.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{surveyForm.Name} <span class='form-text'>Form #{surveyForm.Asset}</span>");

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            UserFeedback.LoadItems(UserFeedbackType.Summary, UserFeedbackType.Detailed, UserFeedbackType.Answered, UserFeedbackType.Disabled);
            UserFeedback.Value = surveyForm.UserFeedback.ToString();

            IsSubmissionsLimitedSelector.ValueAsBoolean = surveyForm.ResponseLimitPerUser.HasValue;
            DurationMinutes.ValueAsInt = surveyForm.DurationMinutes;
            EnableAnonymousSave.ValueAsBoolean = !surveyForm.ResponseLimitPerUser.HasValue && !surveyForm.RequireUserIdentification;
            EnableUserConfidentiality.ValueAsBoolean = surveyForm.EnableUserConfidentiality;
            DisplaySummaryChart.ValueAsBoolean = surveyForm.DisplaySummaryChart;

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}";

            var locked = Organization.Toolkits?.Surveys?.LockUserConfidentiality ?? false;
            EnableUserConfidentiality.Enabled = Identity.IsOperator || !locked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !ValidateIsSubmissionsLimited())
                return;

            var surveyForm = ServiceLocator.SurveySearch.GetSurveyForm(SurveyIdentifier);

            bool requireUserIdentification;
            int? responseLimitPerUser;

            if (IsSubmissionsLimitedSelector.ValueAsBoolean.Value)
            {
                responseLimitPerUser = 1;
                requireUserIdentification = true;
            }
            else
            {
                responseLimitPerUser = null;
                requireUserIdentification = EnableAnonymousSave.ValueAsBoolean == false;
            }

            var userFeedback = UserFeedback.Value.ToEnum<UserFeedbackType>();
            var enableUserConfidentiality = EnableUserConfidentiality.ValueAsBoolean.Value;

            var command = new ChangeSurveyFormSettingsCommand(SurveyIdentifier, userFeedback, requireUserIdentification, false, responseLimitPerUser, DurationMinutes.ValueAsInt, enableUserConfidentiality);
            ServiceLocator.SendCommand(command);

            if (surveyForm.DisplaySummaryChart != DisplaySummaryChart.ValueAsBoolean)
            {
                var command2 = new ChangeSurveyDisplaySummaryChart(SurveyIdentifier, DisplaySummaryChart.ValueAsBoolean.Value);
                ServiceLocator.SendCommand(command2);
            }

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }

        private bool ValidateIsSubmissionsLimited()
        {
            if (!IsSubmissionsLimitedSelector.ValueAsBoolean.Value)
                return true;

            var contacts = ServiceLocator.SurveySearch.GetUsersWithMultiResponseSessions(SurveyIdentifier);

            if (contacts.Count == 0)
                return true;

            var errorMessage = new StringBuilder();
            errorMessage.Append("This form cannot be limited to one submission per respondent because the following users have already submitted the form more than once: ");

            for (int i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];

                if (i > 0)
                    errorMessage.Append(", ");

                errorMessage.Append(contact.UserFullName);
            }

            errorMessage.Append(".");

            EditorStatus.AddMessage(AlertType.Error, errorMessage.ToString());

            return false;
        }
    }
}
