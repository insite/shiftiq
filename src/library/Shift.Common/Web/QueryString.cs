using System.Collections.Generic;
using System.Web;

namespace Shift.Common
{
    public class QueryString
    {
        public string Create(Dictionary<string, string> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return string.Empty;

            var queryParams = new List<string>();

            foreach (var kvp in dictionary)
            {
                string key = HttpUtility.UrlEncode(kvp.Key);
                string value = HttpUtility.UrlEncode(kvp.Value);
                queryParams.Add($"{key}={value}");
            }

            return string.Join("&", queryParams);
        }
    }
}