using System;

using Shift.Common;

namespace InSite.Application.Payments.Read
{
    [Serializable]
    public class QPaymentFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? InvoiceIdentifier { get; set; }
        public Guid? CreatedBy { get; set; }

        public string PaymentStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public bool? Approved { get; set; }

        public int? MinAmount { get; set; }
        public int? MaxAmount { get; set; }

        public DateTimeOffset? PaymentAbortedSince { get; set; }
        public DateTimeOffset? PaymentAbortedBefore { get; set; }
        public DateTimeOffset? PaymentApprovedSince { get; set; }
        public DateTimeOffset? PaymentApprovedBefore { get; set; }
        public DateTimeOffset? PaymentDeclinedSince { get; set; }
        public DateTimeOffset? PaymentDeclinedBefore { get; set; }
        public DateTimeOffset? PaymentStartedSince { get; set; }
        public DateTimeOffset? PaymentStartedBefore { get; set; }

        public bool ExcludeBrokenReferences { get; set; }
        public string TransactionIdentifier { get; set; }
        public int? InvoiceNumber { get; set; }
        public string CustomerEmployer { get; set; }
        public Guid? ProductIdentifier { get; set; }
    }
}
