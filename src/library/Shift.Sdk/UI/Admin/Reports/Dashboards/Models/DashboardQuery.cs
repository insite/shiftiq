using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardQuery
    {
        public string File { get; set; }
        public string FileRaw { get; set; }
        public string Sql { get; set; }
        public string SqlRaw { get; set; }

        [JsonProperty, JsonConverter(typeof(JsonCaseInsensitiveDictionaryConverter<string>))]
        public Dictionary<string, string> Parameters { get; private set; }

        public DashboardQuery()
        {
            Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public DashboardQuery Clone() => new DashboardQuery
        {
            File = File,
            FileRaw = FileRaw,
            Sql = Sql,
            SqlRaw = SqlRaw,
            Parameters = Parameters.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase)
        };
    }
}
