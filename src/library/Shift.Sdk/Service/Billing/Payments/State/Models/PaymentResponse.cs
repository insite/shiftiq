namespace InSite.Domain.Payments
{
    public class PaymentResponse
    {
        public string TransactionIdentifier { get; set; }
        public string AuthorizingMerchantId { get; set; }
        public bool Approved { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
        public string AuthCode { get; set; }
        public string Created { get; set; }
        public string OrderNumber { get; set; }
        public string Type { get; set; }
        public int RiskScore { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public CreditCardResponse Card { get; set; }
    }
}