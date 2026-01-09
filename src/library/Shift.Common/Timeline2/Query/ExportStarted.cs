using System;

namespace Shift.Common
{
    public class ExportStarted
    {
        public string DownloadUrl { get; set; }
        public string ExportFormat { get; set; }
        public string ExportKey { get; set; }

        public DateTimeOffset Expiry { get; set; }
    }
}