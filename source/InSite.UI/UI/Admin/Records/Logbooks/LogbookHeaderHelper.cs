using System;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public static class LogbookHeaderHelper
    {
        public static string GetLogbookHeader(QJournalSetup journalSetup, TimeZoneInfo tz)
        {
            var header = journalSetup.JournalSetupName;

            if (journalSetup.Event != null)
            {
                var scheduledDate = journalSetup.Event.EventScheduledEnd.HasValue && journalSetup.Event.EventScheduledEnd.Value.Date != journalSetup.Event.EventScheduledStart.Date
                    ? $"{journalSetup.Event.EventScheduledStart.FormatDateOnly(tz)} - {journalSetup.Event.EventScheduledEnd.Value.FormatDateOnly(tz)}"
                    : $"{journalSetup.Event.EventScheduledStart.FormatDateOnly(tz)}";


                header += $" <span class='form-text'> for {journalSetup.Event.EventTitle} ({scheduledDate})</span>";
            }

            return header;
        }

    }
}