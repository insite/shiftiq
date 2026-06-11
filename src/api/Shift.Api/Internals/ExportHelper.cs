using System.Collections.Concurrent;
using System.Security.Claims;

using Humanizer;

namespace Shift.Api
{
    public class ExportHelper
    {
        private static readonly ConcurrentDictionary<string, int> _counters = new();

        private readonly string _component;

        private readonly string _entity;

        private readonly string _format;

        private readonly ClaimsPrincipal _user;

        public ExportHelper(string component, string entity, string format, ClaimsPrincipal user)
        {
            _component = component;
            _entity = entity;
            _format = (format ?? "json").ToLower();
            _user = user;
        }

        public string CreateFileName()
        {
            var now = DateTimeOffset.Now;

            var timestamp = GetTimestamp();

            var collection = _entity.Pluralize();

            var name = SanitizeFileName($"{_component} {collection}{timestamp}");

            var extension = "." + _format;

            return $"{name}{extension}";
        }

        public string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return MimeTypes.TryGetValue(extension, out var mimeType)
                ? mimeType
                : "application/octet-stream";
        }

        public string GetFileFormat()
        {
            return _format;
        }

        private string GetTimestamp()
        {
            var timestamp = " ";

            try
            {
                var userId = _user.FindFirst("user_id")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return string.Empty;

                var count = _counters.AddOrUpdate(userId, 1, (key, value) => value + 1);

                var timezone = _user.FindFirst("user_timezone")?.Value ?? "UTC";

                var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);

                var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);

                timestamp += $" {now:yyMMddHHmm}-{count}";
            }
            catch
            {

            }

            return timestamp;
        }

        private static readonly Dictionary<string, string> MimeTypes = new()
        {
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".csv", "text/csv" },
            { ".txt", "text/plain" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".zip", "application/zip" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" }
        };

        private static string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();

            var sanitized = string.Join("", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));

            sanitized = sanitized.Replace(" ", "_");

            if (sanitized.Length > 200)
                sanitized = sanitized.Substring(0, 200);

            return sanitized;
        }
    }
}
