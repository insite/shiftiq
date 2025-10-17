using System;

namespace Shift.Common
{
    public class StartExport
    {
        public string PhysicalFile { get; set; }
        public string ExportFormat { get; set; }
        public string ExportKey { get; set; }

        public DateTimeOffset Expiry { get; set; }
    }
}