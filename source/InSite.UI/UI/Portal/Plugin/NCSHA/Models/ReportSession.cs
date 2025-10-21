using System;
using System.Text;
using System.Text.Json;

namespace InSite.UI.Variant.NCSHA.Reports
{
    public class ReportSession
    {
        public string SessionId { get; set; }
        public DateTimeOffset Expires { get; set; }

        public static ReportSession Decode(string encoded)
        {
            var data = Convert.FromBase64String(encoded);
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<ReportSession>(json);
        }

        public static string Encode(ReportSession session)
        {
            var json = JsonSerializer.Serialize(session);
            var data = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(data);
        }
    }
}