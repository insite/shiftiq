using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DashboardWidget
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public DashboardWidgetType Type { get; set; }

        public DashboardTableQuery Query { get; set; }
        public DashboardChartSettings Chart { get; set; }

        [JsonProperty, JsonConverter(typeof(JsonCaseInsensitiveDictionaryConverter<string>))]
        public Dictionary<string, string> QueryParameters { get; private set; }

        public string Id => "Widget" + Code;

        public DashboardWidget()
        {
            Code = _generator.Next();
            QueryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static RandomStringGenerator _generator = new RandomStringGenerator(RandomStringType.Alphabetic, 6);
    }
}
