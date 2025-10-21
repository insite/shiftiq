using System;
using System.Web.UI;

using InSite.Application.Records.Read;

using Shift.Common;
namespace InSite.UI.Admin.Records.Logbooks.Controls
{
    public partial class LogbookDetailsV2 : UserControl
    {
        public void BindLogbook(Guid journalSetupIdentifier, QJournalSetup journalSetup, TimeZoneInfo tz, bool isValidator)
        {
            var outlineURL = isValidator
                ? $"/ui/admin/records/logbooks/validators/outline?journalsetup={journalSetupIdentifier}"
                : $"/ui/admin/records/logbooks/outline?journalsetup={journalSetupIdentifier}";

            JournalSetupName3.Text = $"<a href=\"{outlineURL}\">{journalSetup.JournalSetupName}</a>";

            var content = ServiceLocator.ContentSearch.GetBlock(journalSetupIdentifier, MultilingualString.DefaultLanguage);
            var title = content?.Title?.Text.Default;

            Title3.Text = !string.IsNullOrEmpty(title) ? title : "None";

            ClassTitle3.Text = !string.IsNullOrEmpty(journalSetup.Event?.EventTitle)
                ? $"<a href=\"/ui/admin/events/classes/outline?event={journalSetup.Event?.EventIdentifier}\">{journalSetup.Event?.EventTitle} </a>"
                : "None";

            if (journalSetup.Event != null)
                ClassScheduled3.Text = $"Scheduled: {GetLocalTime(journalSetup.Event.EventScheduledStart, tz)} - {GetLocalTime(journalSetup.Event.EventScheduledEnd, tz)}";

            AchievementTitle3.Text = !string.IsNullOrEmpty(journalSetup.Achievement?.AchievementTitle)
                ? $"<a href=\"/ui/admin/records/achievements/outline?id={journalSetup.Achievement?.AchievementIdentifier}\">{journalSetup.Achievement?.AchievementTitle} </a>"
                : "None";

            FrameworkTitle3.Text = !string.IsNullOrEmpty(journalSetup.Framework?.FrameworkTitle)
                ? $"<a href=\"/ui/admin/standards/edit?id={journalSetup.FrameworkStandardIdentifier}\">{journalSetup.Framework.FrameworkTitle} </a>"
                : "None";
        }

        private static string GetLocalTime(object obj, TimeZoneInfo tz)
        {
            if (obj == null)
                return null;

            var date = (DateTimeOffset)obj;
            return TimeZones.Format(date, tz, true);
        }
    }
}