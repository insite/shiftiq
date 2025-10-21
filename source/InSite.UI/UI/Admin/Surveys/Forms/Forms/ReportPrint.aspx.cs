using System;
using System.Text;

using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

namespace InSite.Admin.Surveys.Forms.Forms
{
    public partial class ReportPrint : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Survey Report";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack)
                return;

            if (!Guid.TryParse(Request.QueryString["survey"], out var surveyId))
            {
                RedirectToSearch();
                return;
            }

            var survey = ServiceLocator.SurveySearch.GetSurveyState(surveyId);
            if (survey == null || survey.Form.Tenant != Organization.OrganizationIdentifier)
            {
                RedirectToSearch();
                return;
            }

            QResponseAnalysisFilter filter = null;

            try
            {
                var base64 = Request.QueryString["filter"];
                if (!string.IsNullOrEmpty(base64))
                {
                    var bytes = Convert.FromBase64String(base64);
                    var json = Encoding.UTF8.GetString(bytes);

                    filter = JsonConvert.DeserializeObject<QResponseAnalysisFilter>(json);
                }
            }
            catch
            {

            }

            if (filter == null)
            {
                RedirectToReader(surveyId);
                return;
            }

            filter.SurveyFormIdentifier = surveyId;

            SearchResults.Print = true;
            SearchResults.Search(filter);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/surveys/search", true);

        private void RedirectToReader(Guid surveyId) =>
            HttpResponseHelper.Redirect($"/admin/surveys/forms/report?survey={surveyId}", true);
    }
}
