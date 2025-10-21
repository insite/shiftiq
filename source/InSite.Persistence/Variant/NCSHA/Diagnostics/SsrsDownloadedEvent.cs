using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [NcshaHistoryEvent("SQL Server Report", "Report Downloaded", true)]
    public class SsrsDownloadedEvent : SsrsHistoryEvent
    {
        [JsonProperty(PropertyName = "fileFormat")]
        public string FileFormat { get; private set; }

        [JsonProperty(PropertyName = "fileExtension")]
        public string FileExtension { get; private set; }

        public SsrsDownloadedEvent(string code, string name, string format, string fileExtension)
            : base(code, name)
        {
            FileFormat = format;
            FileExtension = fileExtension;
        }
    }
}
