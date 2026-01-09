using System;
using System.Web.UI;

using InSite.Application.Events.Read;

using Shift.Common;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassLocationInfo : UserControl
    {
        public bool ShowSchedule
        {
            get => ScheduleField.Visible;
            set => ScheduleField.Visible = value;
        }

        public bool ShowVenue
        {
            get => (bool)(ViewState[nameof(ShowVenue)] ?? true);
            set => ViewState[nameof(ShowVenue)] = value;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            VenueInfo.Visible = ShowVenue;
            VenueRoomField.Visible = ShowVenue && VenueRoom.Text.HasValue();
        }

        public void Bind(QEvent ev)
        {
            EventScheduledStart.Text = ev.EventScheduledStart.Format(null, true);
            EventScheduledEnd.Text = ev.EventScheduledEnd.Format(null, true, nullValue: "None");

            VenueInfo.Bind(ev.EventIdentifier, ev.VenueLocation);

            VenueRoom.Text = ev.VenueRoom;
        }
    }
}