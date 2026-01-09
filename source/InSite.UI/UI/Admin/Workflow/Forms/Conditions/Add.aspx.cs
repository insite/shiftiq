using System;

using DocumentFormat.OpenXml.Spreadsheet;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Conditions
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier =>
            Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private Guid? DefaultQuestionIdentifier =>
            Guid.TryParse(Request["question"], out var value) ? value : (Guid?)null;

        private string ReturnPanel => Request["returnpanel"] as string;

        private string ReturnTab => Request["returntab"] as string;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        public override void ApplyAccessControl()
        {
            SaveButton.Visible = CanEdit && CanCreate;
            base.ApplyAccessControl();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var result = Detail.GetInputValues();

            var command = new Application.Surveys.Write.AddSurveyCondition(SurveyIdentifier.Value, result.MaskingItem, result.MaskedQuestions);

            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}", true);
        }

        private void Open()
        {
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

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}";

            Detail.SetDefaultInputValues(survey.Form, DefaultQuestionIdentifier, User.TimeZone);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }
    }
}
