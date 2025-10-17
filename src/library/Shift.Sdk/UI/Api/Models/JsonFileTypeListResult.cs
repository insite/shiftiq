using Newtonsoft.Json;

using Shift.Common.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonFileTypeListResult : JsonResult
    {
        #region Construction

        public JsonFileTypeListResult()
            : base("TypeList")
        {
        }

        #endregion

        [JsonProperty(PropertyName = "types")]
        public string[] Types { get; set; }
    }
}