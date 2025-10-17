namespace InSite.Domain.Payments
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string Reference { get; set; }
        public Detail[] Details { get; set; }
        public CreditCardValidation Validation { get; set; }
    }

    public class Detail
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}