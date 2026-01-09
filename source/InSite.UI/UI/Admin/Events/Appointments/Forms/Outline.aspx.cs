using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Events.Appointments.Forms
{
    public partial class Outline : AdminBasePage
    {
        private const string DefaultColor = "Info";
        protected string OutlineUrl => "/ui/admin/events/appointments/outline";
        protected string SearchUrl => "/ui/admin/events/appointments/search";
        protected string DeleteUrl => "/ui/admin/events/appointments/delete";

        private Guid? EventID => Guid.TryParse(Request["event"], out var eventID) ? eventID : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishButton.Click += (s, a) => Publish();
            UnpublishButton.Click += (s, a) => Unpublish();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            var tab = Request["tab"];
            if (tab == "Description")
                DescriptionTab.IsSelected = true;
            else
                TitleTab.IsSelected = true;
        }

        private void LoadData()
        {
            if (EventID == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var ev = ServiceLocator.EventSearch.GetEvent(EventID.Value, x => x.VenueLocation);
            if (ev == null || ev.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var canWrite = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write);
            var canDelete = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete);

            PageHelper.AutoBindHeader(
                this, 
                new BreadcrumbItem("Add New Appointment", "/ui/admin/events/appointments/create", null, null), 
                $"{ev.EventTitle} <span class='form-text'>scheduled {ev.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            EventTitle.Text = ev.EventTitle;
            AppointmentType.Text = ev.AppointmentType.IfNullOrEmpty("N/A");
            EventDescription.Text = ev.EventDescription.IfNullOrEmpty("N/A");
            EventThumbprint.Text = ev.EventIdentifier.ToString();
            EventScheduledStart.Text = ev.EventScheduledStart.Format(null, true);
            EventScheduledEnd.Text = ev.EventScheduledEnd.Format(null, true, nullValue: "None");
            AppointmentCalendarColorBox.Text = ColorBoxHtml(ev.EventCalendarColor ?? DefaultColor);
            AppointmentCalendarColorName.Text = ColorName(ev.EventCalendarColor ?? DefaultColor);

            var isPublished = string.Equals(ev.EventPublicationStatus, PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase);
            var isCancelled = string.Equals(ev.EventSchedulingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase);
            var allowPublish = !isPublished && !isCancelled;

            var content = ContentEventClass.Deserialize(ev.Content);

            if (string.IsNullOrEmpty(content.Title.Default))
                content.Title.Default = ev.EventTitle;

            ContentTitle.LoadData(content.Title);
            ContentDescription.LoadData(content.Description);

            DeleteLink.NavigateUrl = $"{DeleteUrl}?event={EventID}";
            EventTitleLink.HRef = $"/ui/admin/events/appointments/describe?event={EventID}&tab=Title";
            AppointmentTypeLink.HRef = $"/ui/admin/events/appointments/change-appointment-type?event={EventID}";
            AppointmentCalendarColorLink.HRef = $"/ui/admin/events/appointments/change-appointment-type?event={EventID}";
            ChangeEventScheduledStart.HRef = $"/ui/admin/events/appointments/reschedule?event={EventID}";
            ChangeEventScheduledEnd.HRef = $"/ui/admin/events/appointments/reschedule?event={EventID}";
            EditContentTitleLink.NavigateUrl = $"/ui/admin/events/appointments/describe?event={EventID}&tab=Title";
            EditContentDescriptionLink.NavigateUrl = $"/ui/admin/events/appointments/describe?event={EventID}&tab=Description";
            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(EventID.Value, $"/ui/admin/events/appointments/outline?event={EventID}");
            PreviewLink.NavigateUrl = $"/ui/portal/events/appointments/outline?event={EventID}";

            DeleteLink.Visible = canDelete;
            EventTitleLink.Visible = canWrite;
            ChangeEventScheduledStart.Visible = canWrite;
            ChangeEventScheduledEnd.Visible = canWrite;
            NewAppointmentLink.Visible = canWrite;
            EditContentTitleLink.Visible = canWrite;
            EditContentDescriptionLink.Visible = canWrite;
            PublishButton.Visible = allowPublish;
            UnpublishButton.Visible = isPublished;
            BindPublicationPanel(isPublished);
        }

        private string ColorBoxHtml(string itemColor)
        {
            var indicator = itemColor.ToEnumNullable<Indicator>();
            return indicator.HasValue
                ? $"<i class='fas fa-square text-{indicator.GetContextualClass()} me-2'></i>"
                : null;
        }

        private string ColorName(string itemColor)
        {
            var indicator = itemColor.ToEnumNullable<Indicator>();
            return indicator?.GetDescription();
        }

        private void BindPublicationPanel(bool isPublished)
        {
            FormPublicationPanel.Visible = isPublished;

            if (isPublished)
            {
                FormPublicationStatus.Text = "Published";
                FormPublicationStatus.CssClass = "badge bg-success";
                FormPublicationStatusText.Text = "Active";
            }
            else
            {
                FormPublicationStatusText.Text = "Drafted";
            }
        }
        private void Publish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new PublishEvent(EventID.Value, null, null));

            HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private void Unpublish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new UnpublishEvent(EventID.Value));

            HttpResponseHelper.Redirect(Request.RawUrl);
        }
    }
}