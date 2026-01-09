namespace InSite.Domain.Payments
{
    public class CreditCardValidation
    {
        public string ValidationId { get; set; }
        public int Approved { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
        public string AuthCode { get; set; }
        public string TransDate { get; set; }
        public string OrderNumber { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string CvdId { get; set; }
    }
}