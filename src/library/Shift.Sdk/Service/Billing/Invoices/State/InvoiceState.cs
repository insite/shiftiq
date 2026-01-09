using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Sales.Invoices.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Invoices
{
    public class Invoice : AggregateState
    {
        public InvoiceStatus Status { get; set; }
        public Guid Customer { get; set; }
        public Guid? BusinessCustomer { get; set; }
        public Guid? Employee { get; set; }
        public Guid? Issue { get; set; }
        public int? Number { get; set; }
        public DateTimeOffset? Drafted { get; set; }
        public DateTimeOffset? Submitted { get; set; }
        public DateTimeOffset? Paid { get; set; }
        public List<InvoiceItem> Items { get; set; }

        public Invoice()
        {
            Items = new List<InvoiceItem>();
        }

        public static string FormatInvoiceNumber(int invoiceNumber)
        {
            return invoiceNumber.ToString().PadLeft(9, '0');
        }

        public void When(InvoiceCustomerChanged e)
        {
            Customer = e.Customer;
        }

        public void When(InvoiceStatusChanged e)
        {
            Status = e.InvoiceStatus.ToEnum<InvoiceStatus>();
        }

        public void When(InvoicePaidDateChanged e)
        {
            Paid = e.InvocePaidDate;
        }

        public void When(InvoiceDrafted e)
        {
            Status = InvoiceStatus.Drafted;
            Drafted = e.ChangeTime;
            Number = e.Number;
            Customer = e.Customer;
            if (e.Items.IsEmpty())
                Items = new List<InvoiceItem>();
            else
                Items = e.Items.ToList();
        }

        public void When(InvoiceBusinessCustomerChanged e)
        {
            BusinessCustomer = e.BusinessCustomer;
        }

        public void When(InvoiceEmployeeChanged e)
        {
            Employee = e.Employee;
        }

        public void When(InvoiceIssueChanged e)
        {
            Issue = e.Issue;
        }

        public void When(InvoiceItemAdded e)
        {
            Items.Add(e.Item);
        }

        public void When(InvoiceItemChanged e)
        {
            var item = Items.FirstOrDefault(x => x.Identifier == e.Item.Identifier);
            if (item != null)
            {
                item.Description = e.Item.Description;
                item.Price = e.Item.Price;
                item.Quantity = e.Item.Quantity;
                item.Product = e.Item.Product;
            }
        }

        public void When(InvoiceItemRemoved e)
        {
            var item = Items.FirstOrDefault(x => x.Identifier == e.ItemIdentifier);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void When(InvoiceNumberChanged e)
        {
            Number = e.Number;
        }

        public void When(InvoiceSubmitted e)
        {
            Status = InvoiceStatus.Submitted;
            Submitted = e.ChangeTime;
        }

        public void When(InvoicePaid e)
        {
            Status = InvoiceStatus.Paid;
            Paid = e.ChangeTime;
        }

        public void When(InvoicePaymentFailed _)
        {
            Status = InvoiceStatus.PaymentFailed;
        }

        public void When(InvoiceDeleted _) { }

        public void When(InvoiceReferenced _) { }
    }
}
