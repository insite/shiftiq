using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence
{
    public static class VReportHelper
    {
        private class VReportJson
        {
            public string ReportType { get; set; }
            public string ReportTitle { get; set; }
            public string ReportData { get; set; }
            public string ReportDescription { get; set; }
        }

        public static byte[] Serialize(VReport report)
        {
            var data = new VReportJson
            {
                ReportData = report.ReportData,
                ReportDescription = report.ReportDescription,
                ReportType = report.ReportType,
                ReportTitle = report.ReportTitle,
            };
            var json = JsonHelper.JsonExport(data);

            return Encoding.UTF8.GetBytes(json);
        }

        public static VReport Deserialize(string json)
        {
            VReportJson data;

            try
            {
                data = JsonHelper.JsonImport<VReportJson>(json);
            }
            catch (JsonReaderException)
            {
                return null;
            }
            catch (ApplicationError)
            {
                return null;
            }

            return new VReport
            {
                ReportIdentifier = UniqueIdentifier.Create(),
                ReportData = data.ReportData,
                ReportDescription = data.ReportDescription,
                ReportType = data.ReportType,
                ReportTitle = data.ReportTitle
            };
        }
    }
}
