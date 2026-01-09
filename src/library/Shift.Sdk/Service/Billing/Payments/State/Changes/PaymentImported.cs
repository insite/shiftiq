using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Payments
{
    public class PaymentImported : Change
    {
        public Guid Tenant { get; }
        public Guid Invoice { get; }
        public Guid Payment { get; }
        public Guid CreatedBy { get; set; }
        public string OrderNumber { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PaymentStatus Status { get; }

        public DateTimeOffset? Started { get; }
        public DateTimeOffset? Approved { get; }
        public decimal Amount { get; }
        public string CustomerIP { get; }
        public string TransactionId { get; }
        public string ResultCode { get; }
        public string ResultMessage { get; }

        public PaymentImported(
            Guid tenant,
            Guid invoice,
            Guid payment,
            Guid createdBy,
            string orderNumber,
            PaymentStatus status,
            DateTimeOffset? started,
            DateTimeOffset? approved,
            decimal amount,
            string customerIP,
            string transactionId,
            string resultCode,
            string resultMessage
            )
        {
            Tenant = tenant;
            Invoice = invoice;
            Payment = payment;
            CreatedBy = createdBy;
            OrderNumber = orderNumber;
            Status = status;
            Started = started;
            Approved = approved;
            Amount = amount;
            CustomerIP = customerIP;
            TransactionId = transactionId;
            ResultCode = resultCode;
            ResultMessage = resultMessage;
        }
    }
}
