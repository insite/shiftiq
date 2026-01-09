using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ProgressPanelItemClientData
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; }

        [JsonProperty(PropertyName = "type")]
        public abstract string Type { get; }

        public ProgressPanelItemClientData(string id)
        {
            Id = id;
        }
    }
}
