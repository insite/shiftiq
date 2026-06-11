using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [NcshaHistoryEvent("Factbook Analytics", "Chart Downloaded", false)]
    public class ChartDownloadedEvent : ChartHistoryEvent
    {
        [JsonProperty(PropertyName = "fileFormat")]
        public string FileFormat { get; private set; }

        [JsonProperty(PropertyName = "fileExtension")]
        public string FileExtension { get; private set; }

        public ChartDownloadedEvent(string format, string fileExtension)
        {
            FileFormat = format;
            FileExtension = fileExtension;
        }
    }
}
