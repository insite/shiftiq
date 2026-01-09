using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Reports
{
    public class ReportRequest
    {
        #region Fields

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        #endregion

        [JsonProperty(PropertyName = "Search")]
        private object _search;

        public Search<TFilter> GetSearch<TFilter>() where TFilter : Filter => _search as Search<TFilter>;

        public void SetSearch<TFilter>(Search<TFilter> value) where TFilter : Filter => _search = value;

        public void RemoveSearch() => _search = null;

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, jsonSettings);
        }

        public static ReportRequest Deserialize(string json)
        {
            if (json.IsNotEmpty())
            {
                try
                {
                    return JsonConvert.DeserializeObject<ReportRequest>(json, jsonSettings);
                }
                catch
                {

                }
            }

            return null;
        }
    }
}
