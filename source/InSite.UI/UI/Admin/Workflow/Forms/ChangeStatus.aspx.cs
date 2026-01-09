using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ChangeSurveyFormStatusCommand = InSite.Application.Surveys.Write.ChangeSurveyFormStatus;

namespace InSite.Admin.Workflow.Forms
{
    public partial class ChangeStatus : AdminBasePage, IHasParentLinkParameters
    {
        private Guid SurveyIdentifier => Guid.TryParse(Request["form"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CurrentStatus.AutoPostBack = true;
            CurrentStatus.ValueChanged += CurrentStatus_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            if (survey == null
                || survey.Form.Tenant != Organization.Identifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);
            }

            PageHelper.AutoBindHeader(this);
            SurveyDetail.BindSurvey(survey, User.TimeZone, true, false);

            CurrentStatus.Value = survey.Form.Status.GetName();
            ClosedDatePicker.Value = survey.Form.Closed;
            ClosedDate.Text = survey.Form.Closed.HasValue ? survey.Form.Closed.Value.Format(User.TimeZone, true) : "None";
            OpenedDate.Text = survey.Form.Opened.HasValue ? survey.Form.Opened.Value.Format(User.TimeZone, true) : "None";

            SetControlVisibility();

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}";
        }

        private void CurrentStatus_ValueChanged(object sender, EventArgs e)
        {
            if (!ClosedDatePicker.Value.HasValue && CurrentStatus.Value == SurveyFormStatus.Closed.GetName())
                ClosedDatePicker.Value = DateTimeOffset.UtcNow;

            SetControlVisibility();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);
            var newSurveyFormStatus = CurrentStatus.Value.ToEnum<SurveyFormStatus>();

            var newOpenedDate = survey.Form.Opened
                ?? (newSurveyFormStatus == SurveyFormStatus.Opened ? DateTimeOffset.UtcNow : (DateTimeOffset?)null);

            var newClosedDate = newSurveyFormStatus != SurveyFormStatus.Archived
                ? ClosedDatePicker.Value
                : survey.Form.Closed;

            var commands = new List<Command>();

            if (survey.Form.Status != newSurveyFormStatus)
                commands.Add(new ChangeSurveyFormStatusCommand(SurveyIdentifier, newSurveyFormStatus));

            if (newOpenedDate != survey.Form.Opened || newClosedDate != survey.Form.Closed)
                commands.Add(new ChangeSurveyFormSchedule(SurveyIdentifier, newOpenedDate, newClosedDate));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}");
        }

        private void SetControlVisibility()
        {
            var canModifyCloseDate = CurrentStatus.Value != SurveyFormStatus.Archived.GetName();

            ClosedDatePicker.Visible = canModifyCloseDate;
            ClosedDate.Visible = !canModifyCloseDate;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }
    }
}
