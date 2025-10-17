using System.Collections.Generic;
using System.IO;

namespace Shift.Common
{
    public static class MimeMapping
    {
        private static readonly Dictionary<string, string> MimeMappings = new Dictionary<string, string>()
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/msword"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"},
            {".html", "text/html"},
            {".htm", "text/html"},
            {".css", "text/css"},
            {".js", "application/javascript"},
            {".json", "application/json"},
            {".xml", "application/xml"},
            {".zip", "application/zip"},
            {".mp3", "audio/mpeg"},
            {".mp4", "video/mp4"},
            {".avi", "video/x-msvideo"},
            {".mov", "video/quicktime"},
            {".svg", "image/svg+xml"},
            {".ico", "image/x-icon"}
        };

        public static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return MimeMappings.TryGetValue(extension, out string mimeType)
                ? mimeType
                : "application/octet-stream";
        }
    }
}
