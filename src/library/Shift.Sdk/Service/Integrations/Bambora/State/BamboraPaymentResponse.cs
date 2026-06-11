using Newtonsoft.Json;

namespace InSite.Domain.Integrations.Bambora
{
    public class BamboraPaymentResponse
    {
        public string id { get; set; }
        public int authorizing_merchant_id { get; set; }
        public int approved { get; set; }
        public int message_id { get; set; }
        public string message { get; set; }
        public string auth_code { get; set; }
        public string created { get; set; }
        public string order_number { get; set; }
        public string type { get; set; }
        public decimal risk_score { get; set; }
        public decimal amount { get; set; }
        public string payment_method { get; set; }

        public static BamboraPaymentResponse Deserialize(string content)
        {
            return JsonConvert.DeserializeObject<BamboraPaymentResponse>(content);
        }
    }
}
