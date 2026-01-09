using System;
using System.Web.UI;

using InSite.Application.Events.Read;

using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassSummaryInfo : UserControl
    {
        public bool ShowHeading
        {
            get => ClassHeadingDiv.Visible;
            set => ClassHeadingDiv.Visible = value;
        }

        public void Bind(QEvent ev)
        {
            var achievement = ev.AchievementIdentifier.HasValue 
                ? ServiceLocator.AchievementSearch.GetAchievement(ev.AchievementIdentifier.Value) 
                : null;

            EventTitle.Text = ev.EventTitle;
            EventLink.HRef = $"/ui/admin/events/classes/outline?event={ev.EventIdentifier}";
            AchievementTitle.Text = achievement != null ? $"<a href=\"/ui/admin/records/achievements/outline?id={achievement.AchievementIdentifier}\">{achievement.AchievementTitle}</a>" : "None";
            IsPublished.Visible = string.Equals(ev.EventPublicationStatus, PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase);
            EventSchedulingStatus.Text = ev.EventSchedulingStatus ?? "None";
        }
    }
}