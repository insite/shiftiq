using System;
using System.Text;

using Shift.Common;

namespace InSite.UI.Portal.Events.Classes
{
    public class CalendarHelper
    {
        public string GenerateGoogleCalendarLink(
            DateTime startDateTime,
            DateTime endDateTime,
            string eventTitle,
            string location = null,
            string description = null)
        {
            // Format dates as Google Calendar expects (yyyyMMddTHHmmssZ)
            string startTime = startDateTime.ToUniversalTime().ToString("yyyyMMddTHHmmss") + "Z";
            string endTime = endDateTime.ToUniversalTime().ToString("yyyyMMddTHHmmss") + "Z";

            // Build the Google Calendar URL
            string calendarUrl = "https://calendar.google.com/calendar/render?action=TEMPLATE";
            calendarUrl += $"&text={Uri.EscapeDataString(eventTitle)}";
            calendarUrl += $"&dates={startTime}/{endTime}";

            if (!string.IsNullOrEmpty(location))
                calendarUrl += $"&location={Uri.EscapeDataString(location)}";

            if (!string.IsNullOrEmpty(description))
                calendarUrl += $"&details={Uri.EscapeDataString(description)}";

            return $"<a class='btn btn-sm btn-primary' target='_blank' href='{calendarUrl}'><i class='fa-solid fa-calendar-circle-plus me-1'></i>Add to Google Calendar</a>";
        }

        public string GenerateOffice365CalendarLink(
            DateTime startDateTime,
            DateTime endDateTime,
            string eventTitle,
            string location = null,
            string description = null)
        {
            // Convert to UTC and format as ISO 8601
            string startTime = startDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endTime = endDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            // URL encode the parameters
            string encodedTitle = Uri.EscapeDataString(eventTitle);
            string encodedStartTime = Uri.EscapeDataString(startTime);
            string encodedEndTime = Uri.EscapeDataString(endTime);

            // Build the Office 365 calendar URL
            string calendarUrl = $"https://outlook.office.com/calendar/0/deeplink/compose?" +
                                $"subject={encodedTitle}" +
                                $"&startdt={encodedStartTime}" +
                                $"&enddt={encodedEndTime}" +
                                $"&path=/calendar/action/compose";

            if (!string.IsNullOrEmpty(location))
                calendarUrl += $"&location={Uri.EscapeDataString(location)}";

            if (!string.IsNullOrEmpty(description))
                calendarUrl += $"&body={Uri.EscapeDataString(description)}";

            return $"<a class='btn btn-sm btn-primary' target='_blank' href='{calendarUrl}'><i class='fa-solid fa-calendar-circle-plus me-1'></i>Add to Outlook Calendar</a>";
        }

        public string GenerateIcsDownloadLink(
            DateTime startDateTime,
            DateTime endDateTime,
            string eventTitle,
            string location = null,
            string description = null)
        {
            string icsContent = GenerateIcsContent(startDateTime, endDateTime, eventTitle, location, description);

            // Convert to base64 for data URI
            byte[] bytes = Encoding.UTF8.GetBytes(icsContent);
            string base64 = Convert.ToBase64String(bytes);

            // Create data URI
            string dataUri = $"data:text/calendar;base64,{base64}";
            string filename = GenerateIcsFileName(eventTitle, startDateTime);

            return $"<a class='btn btn-sm btn-default' target='_blank' download='{filename}' href='{dataUri}'><i class='fa-solid fa-download me-1'></i>Download ICS</a>";
        }

        public string GenerateIcsFileName(string eventTitle, DateTime startDateTime)
        {
            string sanitized = StringHelper.Sanitize(eventTitle, '-');
            return $"{sanitized}_{startDateTime:yyyy-MM-dd}.ics";
        }

        private string GenerateIcsContent(
            DateTime startDateTime,
            DateTime endDateTime,
            string eventTitle,
            string location = null,
            string description = null)
        {
            // Format dates as UTC in ICS format (yyyyMMddTHHmmssZ)
            string startTime = startDateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
            string endTime = endDateTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

            // Generate unique identifier
            string uid = Guid.NewGuid().ToString();

            // Build ICS content
            var ics = new StringBuilder();
            ics.AppendLine("BEGIN:VCALENDAR");
            ics.AppendLine("VERSION:2.0");
            ics.AppendLine("PRODID:-//Your Company//Your App//EN");
            ics.AppendLine("BEGIN:VEVENT");
            ics.AppendLine($"UID:{uid}");
            ics.AppendLine($"DTSTAMP:{timestamp}");
            ics.AppendLine($"DTSTART:{startTime}");
            ics.AppendLine($"DTEND:{endTime}");
            ics.AppendLine($"SUMMARY:{EscapeIcsText(eventTitle)}");

            if (!string.IsNullOrEmpty(location))
                ics.AppendLine($"LOCATION:{EscapeIcsText(location)}");

            if (!string.IsNullOrEmpty(description))
                ics.AppendLine($"DESCRIPTION:{EscapeIcsText(description)}");

            ics.AppendLine("STATUS:CONFIRMED");
            ics.AppendLine("SEQUENCE:0");
            ics.AppendLine("END:VEVENT");
            ics.AppendLine("END:VCALENDAR");

            return ics.ToString();
        }

        private string EscapeIcsText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Escape special characters according to ICS specification
            return text
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\n", "\\n")
                .Replace("\r", "");
        }
    }
}