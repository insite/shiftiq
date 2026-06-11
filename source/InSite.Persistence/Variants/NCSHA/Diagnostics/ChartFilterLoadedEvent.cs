using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [NcshaHistoryEvent("Factbook Analytics", "Filter Loaded", false)]
    public class ChartFilterLoadedEvent : ChartHistoryEvent
    {
        [JsonProperty(PropertyName = "filterId")]
        public Guid FilterId { get; private set; }

        [JsonProperty(PropertyName = "filterName")]
        public string FilterName { get; private set; }

        public ChartFilterLoadedEvent(Guid filterId, string filterName)
        {
            FilterId = filterId;
            FilterName = filterName;
        }
    }
}
