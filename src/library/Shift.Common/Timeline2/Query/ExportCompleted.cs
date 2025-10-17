namespace Shift.Common
{
    public class ExportCompleted
    {
        public string PhysicalFile { get; set; }

        public string ExportFormat { get; set; }
        public string ExportKey { get; set; }
        public string ExportName { get; set; }

        public string ContentDisposition
        {
            get
            {
                return $"attachment; filename={ExportName}.{ExportFormat}";
            }
        }

        public string ContentType
        {
            get
            {
                if (ExportFormat == "json")
                    return "application/json";

                if (ExportFormat == "csv")
                    return "text/csv";

                return "text/plain";
            }
        }
    }
}