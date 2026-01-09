using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms
{
    public partial class ConfigureWorkflow : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier => Guid.TryParse(Request["form"], out var result) ? result : (Guid?)null;

        private string ReturnPanel => Request["returnpanel"] as string;

        private string ReturnTab => Request["returntab"] as string;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OpenIssueEnabled.AutoPostBack = true;
            OpenIssueEnabled.CheckedChanged += (s, a) => OnCheckedChanged();

            IssueType.Settings.CollectionName = CollectionName.Cases_Classification_Type;
            IssueType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;

            IssueType.AutoPostBack = true;
            IssueType.ValueChanged += (x, y) =>
            {
                IssueStatus.IssueType = IssueType.Value;
                IssueStatus.RefreshData();
            };

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value) : null;
            if (survey == null
                || survey.Form.Tenant != Organization.Identifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Form #{survey.Form.Asset}</span>");

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            var messages = survey.Form.Messages;

            SurveyInvitationMessageIdentifier.Filter.Type = MessageTypeName.Invitation;
            SurveyInvitationMessageIdentifier.Value = messages.FirstOrDefault(x => x.Type == SurveyMessageType.Invitation)?.Identifier;

            ResponseStartedAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            ResponseStartedAdministratorMessageIdentifier.Value = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseStarted)?.Identifier;

            ResponseCompletedAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            ResponseCompletedAdministratorMessageIdentifier.Value = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseCompleted)?.Identifier;

            ResponseCompletedRespondentMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            ResponseCompletedRespondentMessageIdentifier.Value = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseConfirmed)?.Identifier;

            var openIssueEnabled = survey.WorkflowConfiguration != null;
            OpenIssueEnabled.Checked = openIssueEnabled;

            OnCheckedChanged();
            
            if (openIssueEnabled)
            {
                IssueType.Value = survey.WorkflowConfiguration.IssueType;
                IssueStatus.IssueType = survey.WorkflowConfiguration.IssueType;
                IssueStatus.RefreshData();
                IssueStatus.Value = survey.WorkflowConfiguration.IssueStatusIdentifier.ToString();
                IssueAdministratorIdentifier.Value = survey.WorkflowConfiguration.AdministratorUserIdentifier;
                IssueOwnerIdentifier.Value = survey.WorkflowConfiguration.OwnerUserIdentifier;
            }

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}";
        }

        private void OnCheckedChanged()
        {
            var isChecked = OpenIssueEnabled.Checked;
            IssueWorkflowContainer.Visible = isChecked;
            OpenIssueEnabledWarning.Visible = isChecked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var messages = new List<SurveyMessage>();

            if (SurveyInvitationMessageIdentifier.Value.HasValue)
                messages.Add(new SurveyMessage { Identifier = SurveyInvitationMessageIdentifier.Value.Value, Type = SurveyMessageType.Invitation });

            if (ResponseStartedAdministratorMessageIdentifier.Value.HasValue)
                messages.Add(new SurveyMessage { Identifier = ResponseStartedAdministratorMessageIdentifier.Value.Value, Type = SurveyMessageType.ResponseStarted });

            if (ResponseCompletedAdministratorMessageIdentifier.Value.HasValue)
                messages.Add(new SurveyMessage { Identifier = ResponseCompletedAdministratorMessageIdentifier.Value.Value, Type = SurveyMessageType.ResponseCompleted });

            if (ResponseCompletedRespondentMessageIdentifier.Value.HasValue)
                messages.Add(new SurveyMessage { Identifier = ResponseCompletedRespondentMessageIdentifier.Value.Value, Type = SurveyMessageType.ResponseConfirmed });

            var command = new ChangeSurveyFormMessages(SurveyIdentifier.Value, messages.ToArray());

            ServiceLocator.SendCommand(command);

            OnIssueWorkflowConfigured();

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}");
        }

        private void OnIssueWorkflowConfigured()
        {
            var surveyId = SurveyIdentifier.Value;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyId);

            if (!OpenIssueEnabled.Checked)
            {
                if (survey.WorkflowConfiguration != null)
                    ServiceLocator.SendCommand(new ConfigureSurveyWorkflow(surveyId, null));
            }
            else
            {
                var configuration = new SurveyWorkflowConfiguration
                {
                    IssueType = IssueType.Value,
                    IssueStatusIdentifier = IssueStatus.ValueAsGuid,
                    AdministratorUserIdentifier = IssueAdministratorIdentifier.Value,
                    OwnerUserIdentifier = IssueOwnerIdentifier.Value
                };

                if (!IsEqual(survey.WorkflowConfiguration, configuration))
                    ServiceLocator.SendCommand(new ConfigureSurveyWorkflow(surveyId, configuration));
            }
        }

        private bool IsEqual(SurveyWorkflowConfiguration before, SurveyWorkflowConfiguration after)
            => StringHelper.Equals(ServiceLocator.Serializer.Serialize(before), ServiceLocator.Serializer.Serialize(after));

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }
    }
}
