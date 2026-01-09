using System.Web.UI;

using InSite.Application.Events.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class ExamInfoSummary : UserControl
    {
        public void LoadData(QEvent @event, InSite.Application.Contacts.Read.VGroup venue, bool showAchievement = true, bool showVenue = true, bool showSchedule = true)
        {
            var achievement = @event.AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(@event.AchievementIdentifier.Value) : null;

            AchievementField.Visible = showAchievement;
            if (AchievementField.Visible)
                AchievementTitle.Text = achievement != null 
                    ? $"<a href=\"/ui/admin/records/achievements/outline?id={achievement.AchievementIdentifier}\">{achievement.AchievementTitle}</a>"
                    : "None";

            EventTitle.Text = @event.EventTitle;
            ExamType.Text = @event.ExamType;
            ExamFormat.Text = @event.EventFormat;
            EventStatus.Text = GetStatus(@event);
            EventLink.HRef = $"/ui/admin/events/exams/outline?event={@event.EventIdentifier}";

            //1 - Physical Address
            Venue.Visible = showVenue;
            if (Venue.Visible)
                Venue.BindVenue(@event, venue, AddressType.Physical, "Physical Location", "", "Venue", false);

            VenueRoomField.Visible = @event.VenueRoom.HasValue() && showVenue;
            if (VenueRoomField.Visible)
                VenueRoom.Text = @event.VenueRoom;

            Schedule.Visible = showSchedule;
            if (Schedule.Visible)
            {
                EventScheduledStart.Text = @event.EventScheduledStart.Format(null, true);
            }
        }

        private static string GetStatus(QEvent @event)
        {
            var status = @event.EventPublicationStatus ?? PublicationStatus.Drafted.GetDescription();

            if (@event.EventSchedulingStatus.HasValue())
                status += $" ({@event.EventSchedulingStatus})";

            return status;
        }
    }
}