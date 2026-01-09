using Newtonsoft.Json;

namespace Shift.Common.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonErrorResult : JsonResult
    {
        public JsonErrorResult(string message)
            : base("ERROR")
        {
            Message = message;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; }
    }
}