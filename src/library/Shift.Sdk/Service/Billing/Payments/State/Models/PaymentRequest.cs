namespace InSite.Domain.Payments
{
    public class PaymentInput
    {
        public PaymentInput(string orderNumber, decimal amount, UnmaskedCreditCard card, BillingAddress billingAddress, string customerIP)
        {
            OrderNumber = orderNumber;
            Amount = amount;
            Card = card;
            BillingAddress = billingAddress;
            CustomerIP = customerIP;
        }

        public string OrderNumber { get; }

        public decimal Amount { get; }

        public UnmaskedCreditCard Card { get; }

        public BillingAddress BillingAddress { get; }

        public string CustomerIP { get; }

        public PaymentRequest CreateRequest()
        {
            var card = new MaskedCreditCard
            {
                Name = Card.CardholderName,
                ExpiryMonth = Card.ExpiryMonth,
                ExpiryYear = Card.ExpiryYear
            };

            card.SetNumber(Card.CardNumber);
            card.SetCvd(Card.SecurityCode);

            return new PaymentRequest(OrderNumber, Amount, card, BillingAddress, CustomerIP);
        }
    }

    public class PaymentRequest
    {
        public PaymentRequest() { }

        public PaymentRequest(string orderNumber, decimal amount, MaskedCreditCard card, BillingAddress billingAddress, string customerIP)
        {
            OrderNumber = orderNumber;
            Amount = amount;
            Card = card;
            BillingAddress = billingAddress;
            CustomerIP = customerIP;
        }

        public string OrderNumber { get; }

        public decimal Amount { get; }

        public MaskedCreditCard Card { get; }

        public BillingAddress BillingAddress { get; }

        public string CustomerIP { get; }
    }
}