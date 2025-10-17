using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ProgressPanelTriggerClientData
    {
        [JsonProperty(PropertyName = "type")]
        public abstract string Type { get; }
    }
}
