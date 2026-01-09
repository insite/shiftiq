using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Gradebooks.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? GradebookID => Guid.TryParse(Request.QueryString["gradebook"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var queryGradebook = GradebookID.HasValue ? ServiceLocator.RecordSearch.GetGradebook(GradebookID.Value, x => x.Event, x => x.Achievement) : null;
                if (queryGradebook == null || queryGradebook.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                {
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");
                    return;
                }

                var dataGradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookID.Value);

                var title = queryGradebook.GradebookTitle;

                if (queryGradebook.Event != null)
                    title += $" <span class='form-text'> for {queryGradebook.Event.EventTitle} ({GetLocalTime(queryGradebook.Event.EventScheduledStart)} - {GetLocalTime(queryGradebook.Event.EventScheduledEnd)})</span>";

                PageHelper.AutoBindHeader(this, null, title);

                GradebookDetails.BindGradebook(queryGradebook, User.TimeZone);

                SetupDownloadSection();

                CancelLink.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookID}&panel=gradebook";
            }
        }

        private void SetupDownloadSection()
        {
            FileName.Text = string.Format("gradebook-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
                SendJson();

            else if (fileFormat == "CSV")
                SendCsv();
        }

        private void SendJson()
        {
            var data = GradebookHelper.Serialize(GradebookID.Value);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        private void SendCsv()
        {
            var csv = ServiceLocator.RecordSearch.BuildGradebookReport(GradebookID.Value);
            var data = System.Text.Encoding.UTF8.GetBytes(csv);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "csv");
            else
                Response.SendFile(FileName.Text, "csv", data);
        }

        private static string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookID}&panel=gradebook"
                : null;
        }
    }
}
