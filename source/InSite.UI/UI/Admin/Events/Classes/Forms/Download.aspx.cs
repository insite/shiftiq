using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Events.Classes.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventIdentifier =>
            Guid.TryParse(Request.QueryString["event"], out var value) ? value : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += (s, a) => OnDownload();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation)
                : null;

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            FileName.Text = string.Format("class-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void OnDownload()
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                var data = ClassEventHelper.Serialize(ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.Seats));

                if (CompressionMode.Value == "ZIP")
                    SendZipFile(data, FileName.Text, "json");
                else
                    Response.SendFile(FileName.Text, "json", data);
            }
        }
    }
}