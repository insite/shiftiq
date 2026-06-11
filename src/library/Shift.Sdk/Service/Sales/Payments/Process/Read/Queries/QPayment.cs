using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;
using InSite.Application.Invoices.Read;
using InSite.Application.Registrations.Read;

namespace InSite.Application.Payments.Read
{
    public class QPayment
    {
        public Guid InvoiceIdentifier { get; set; }
        public Guid PaymentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string TransactionId { get; set; }
        public string CardNumber { get; set; }
        public string CardholderName { get; set; }

        public string CustomerIP { get; set; }
        public string PaymentStatus { get; set; }
        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTimeOffset? PaymentAborted { get; set; }
        public DateTimeOffset? PaymentApproved { get; set; }
        public DateTimeOffset? PaymentDeclined { get; set; }
        public DateTimeOffset? PaymentStarted { get; set; }

        public Guid CreatedBy { get; set; }

        public virtual VUser CreatedByUser { get; set; }
        public virtual VInvoice CreatedInvoice { get; set; }

        public virtual ICollection<QRegistration> Registrations { get; set; } = new HashSet<QRegistration>();
    }
}
