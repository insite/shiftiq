using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Rename : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier => Guid.TryParse(Request["form"], out var result) ? result : (Guid?)null;

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

            SurveyDetail.BindSurvey(survey, User.TimeZone, false);

            Name.Text = survey.Form.Name;
            Hook.Text = survey.Form.Hook;

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value);

            if (!StringHelper.Equals(survey.Form.Name, Name.Text))
            {
                var rename = new Application.Surveys.Write.RenameSurveyForm(SurveyIdentifier.Value, Name.Text);
                ServiceLocator.SendCommand(rename);
            }

            if (!StringHelper.Equals(survey.Form.Hook, Hook.Text))
            {
                var rehook = new Application.Surveys.Write.ChangeSurveyHook(SurveyIdentifier.Value, Hook.Text);
                ServiceLocator.SendCommand(rehook);
            }

            Course2Store.ClearCache(Organization.Identifier);

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }
    }
}
