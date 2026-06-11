using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Sales.Invoices.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Invoice();

        public Invoice Data => (Invoice)State;

        public void AddInvoiceItem(InvoiceItem item)
        {
            Apply(new InvoiceItemAdded(item));
        }

        public void ChangeInvoiceCustomer(Guid customer)
        {
            Apply(new InvoiceCustomerChanged(customer));
        }

        public void ChangeInvoiceStatus(string status)
        {
            Apply(new InvoiceStatusChanged(status));
        }

        public void ChangeInvoicePaidDate(DateTimeOffset? dateTimeOffset)
        {
            Apply(new InvoicePaidDateChanged(dateTimeOffset));
        }

        public void ChangeInvoiceNumber(int number)
        {
            Apply(new InvoiceNumberChanged(number));
        }

        public void ChangeInvoiceItem(InvoiceItem item)
        {
            Apply(new InvoiceItemChanged(item));
        }

        public void DraftInvoice(Guid organization, int number, Guid customer, InvoiceItem[] items)
        {
            Apply(new InvoiceDrafted(organization, number, customer, items));
        }


        public void ChangeInvoiceBusinessCustomer(Guid? group)
        {
            Apply(new InvoiceBusinessCustomerChanged(group));
        }

        public void ChangeInvoiceEmployee(Guid employee)
        {
            Apply(new InvoiceEmployeeChanged(employee));
        }

        public void ChangeInvoiceIssue(Guid issue)
        {
            Apply(new InvoiceIssueChanged(issue));
        }

        public void RemoveInvoiceItem(Guid item)
        {
            Apply(new InvoiceItemRemoved(item));
        }

        public void SubmitInvoice()
        {
            Apply(new InvoiceSubmitted());
        }

        public void PayInvoice(DateTimeOffset? paid)
        {
            Apply(new InvoicePaid(paid));
        }

        public void FailInvoicePayment()
        {
            Apply(new InvoicePaymentFailed());
        }

        public void DeleteInvoice()
        {
            Apply(new InvoiceDeleted());
        }

        public void ReferenceInvoice(Guid? referencedInvoice)
        {
            Apply(new InvoiceReferenced(referencedInvoice));
        }
    }
}
