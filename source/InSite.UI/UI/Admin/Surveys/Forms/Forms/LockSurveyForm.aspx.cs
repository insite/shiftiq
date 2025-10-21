using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Surveys.Forms.Forms
{
    public partial class LockSurveyForm : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier => Guid.TryParse(Request["survey"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockButton.Click += LockButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value) : null;

            if (survey == null || survey.Form.Tenant != CurrentSessionState.Identity.Organization.Identifier)
                HttpResponseHelper.Redirect($"/ui/admin/surveys/search", true);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Survey Form #{survey.Form.Asset}</span>");

            SurveyDetail.BindSurvey(survey, User.TimeZone);

            CancelButton.NavigateUrl = $"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}";
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value);

            if (survey != null)
                ServiceLocator.SendCommand(new Application.Surveys.Write.LockSurveyForm(SurveyIdentifier.Value, DateTimeOffset.UtcNow));

            HttpResponseHelper.Redirect($"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"survey={SurveyIdentifier}"
                : null;
        }
    }
}
