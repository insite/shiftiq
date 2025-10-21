using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Persistence
{
    public static class EvaluationCountHelper
    {
        public static int? GetCount(string json, string name)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var map = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

            int value;
            return map.TryGetValue(name, out value) ? value : (int?)null;
        }

        public static string SetCount(string json, string name, int? value)
        {
            var map = !string.IsNullOrEmpty(json) ?  JsonConvert.DeserializeObject<Dictionary<string, int>>(json) : new Dictionary<string, int>();

            if (value == null)
                map.Remove(name);
            else
                map[name] = value.Value;

            return map.Count > 0 ? JsonConvert.SerializeObject(map) : null;
        }
    }
}
