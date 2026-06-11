using InSite.Domain.Payments;

using Newtonsoft.Json;

namespace InSite.Domain.Integrations.Bambora
{
    public class BamboraPaymentRequest
    {
        public string order_number { get; set; }
        public decimal amount { get; set; }
        public string payment_method => "card";
        public string customer_ip { get; set; }
        public BamboraCreditCard card { get; set; }
        public BamboraBillingAddress billing { get; set; }

        public static BamboraPaymentRequest Create(PaymentRequest original)
        {
            var clone = new BamboraPaymentRequest
            {
                order_number = original.OrderNumber,
                amount = original.Amount,
                customer_ip = original.CustomerIP,
                card = new BamboraCreditCard
                {
                    number = original.Card.GetNumber(),
                    name = original.Card.Name,
                    expiry_month = original.Card.ExpiryMonth.ToString("00"),
                    expiry_year = original.Card.ExpiryYear.ToString(),
                    cvd = original.Card.GetCvd()
                }
            };

            if (original.BillingAddress != null)
                clone.billing = new BamboraBillingAddress
                {
                    name = original.BillingAddress.Name,
                    email_address = original.BillingAddress.Email,
                    phone_number = original.BillingAddress.Phone,
                    address_line1 = original.BillingAddress.Address,
                    city = original.BillingAddress.City,
                    province = original.BillingAddress.Province,
                    postal_code = original.BillingAddress.PostalCode,
                    country = original.BillingAddress.Country
                };

            return clone;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
