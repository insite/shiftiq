using System;

using Shift.Constant;

namespace InSite.Domain.Payments
{
    public class Payment
    {
        public PaymentStatus Status { get; set; }
        public Guid CreatedBy { get; set; }

        public Payment(string orderNumber, decimal amount, MaskedCreditCard card, BillingAddress billing, string customerIP)
        {
            Request = new PaymentRequest(orderNumber,amount, card, billing, customerIP);
            Response = new PaymentResponse();
            Error = new ErrorResponse();
        }

        public PaymentRequest Request { get; set; }
        public PaymentResponse Response { get; set; }
        public ErrorResponse Error { get; set; }
    }
}