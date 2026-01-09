using System;
using System.Web.UI;

using InSite.Admin.Workflow.Forms.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var survey = SurveyID.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value) : null;
            if (survey == null || survey.Form.Tenant != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search");
                return;
            }

            PageHelper.AutoBindHeader(this);

            SetupDownloadSection(survey);

            CancelLink.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyID}";
        }

        private void SetupDownloadSection(SurveyState survey)
        {
            SurveyDetail.BindSurvey(survey, User.TimeZone);

            FileName.Text = survey.Form.Name;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                SendJson();
            }
        }

        private void SendJson()
        {
            var resolver = new DownloadContractResolver();
            var json = FormSerializer.Serialize(SurveyID.Value, resolver);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(json, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", json);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyID}"
                : null;
        }
    }
}
