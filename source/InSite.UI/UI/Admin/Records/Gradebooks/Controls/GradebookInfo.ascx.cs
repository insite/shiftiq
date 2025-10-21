using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class GradebookInfo : UserControl
    {
        public void BindGradebook(QGradebook gradebook, TimeZoneInfo tz)
        {
            GradebookLink.HRef = $"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}";
            GradebookTitle.Text = gradebook.GradebookTitle;
            GradebookCreated.Text = gradebook.GradebookCreated.Format(tz);
            LockedField.Visible = gradebook.IsLocked;

            ClassTitle.Text = gradebook.Event != null ?
                $"<a href=\"/ui/admin/events/classes/outline?event={gradebook.Event?.EventIdentifier}\">{gradebook.Event?.EventTitle} </a>" : "None";

            if (gradebook.Event != null)
            {
                ClassScheduled.Text = $"Scheduled: {GetLocalTime(gradebook.Event.EventScheduledStart, tz)} - {GetLocalTime(gradebook.Event.EventScheduledEnd, tz)}";

                BindClassInstructors(gradebook.GradebookIdentifier);
            }

            AchievementTitle.Text = gradebook.Achievement != null ?
                $"<a href=\"/ui/admin/records/achievements/outline?id={gradebook.Achievement?.AchievementIdentifier}\">{gradebook.Achievement?.AchievementTitle} </a>" : "None";

            var gradebookType = gradebook.GradebookType.ToEnum<GradebookType>();
            Scores.Checked = gradebookType == GradebookType.Scores || gradebookType == GradebookType.ScoresAndStandards;
            Standards.Checked = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;

            StandardField.Visible = gradebookType == GradebookType.Standards || gradebookType == GradebookType.ScoresAndStandards;
            FrameworkTitle.Text = gradebook.FrameworkIdentifier.HasValue ?
                $"<a href=\"/ui/admin/standards/edit?id={gradebook.FrameworkIdentifier}\">{StandardSearch.Select(gradebook.FrameworkIdentifier.Value)?.ContentTitle} </a>" : "None";
        }

        private void BindClassInstructors(Guid gradebookId)
        {
            var instructors = ServiceLocator.EventSearch.GetAttendees(
                            new QEventAttendeeFilter { GradebookIdentifier = gradebookId, ContactRole = "Instructor" },
                            null, null, x => x.Person.User);

            ClassInstructorsField.Visible = instructors.Any();

            ClassInstructors.DataSource = instructors;
            ClassInstructors.DataBind();
        }

        private static string GetLocalTime(DateTimeOffset? item, TimeZoneInfo tz)
        {
            return item.FormatDateOnly(tz, nullValue: string.Empty);
        }
    }
}