using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject]
    public class ContentSettings
    {
        public HashSet<string> Locked { get; set; } = new HashSet<string>();

        public static ContentSettings Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentSettings()
                : JsonConvert.DeserializeObject<ContentSettings>(json);
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
    }
}