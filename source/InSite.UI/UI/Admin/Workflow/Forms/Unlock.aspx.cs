using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Unlock : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier => Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UnlockButton.Click += UnlockButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value) : null;

            if (survey == null || survey.Form.Tenant != CurrentSessionState.Identity.Organization.Identifier)
                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);

            PageHelper.AutoBindHeader(this);

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}";
        }

        private void UnlockButton_Click(object sender, EventArgs e)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value);

            if (survey != null)
                ServiceLocator.SendCommand(new Application.Surveys.Write.UnlockSurveyForm(SurveyIdentifier.Value, DateTimeOffset.UtcNow));

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
