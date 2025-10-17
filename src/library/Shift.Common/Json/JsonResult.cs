using Newtonsoft.Json;

namespace Shift.Common.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonResult
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        public JsonResult(string type)
        {
            Type = type;
        }
    }
}
