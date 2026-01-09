using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Invoices;
using InSite.Domain.Sales.Invoices.Changes;

namespace InSite.Application.Invoices.Read
{
    public interface IInvoiceStore
    {
        void InsertInvoice(InvoiceDrafted e);
        void UpdateInvoice(InvoiceCustomerChanged e);
        void UpdateInvoice(InvoiceEmployeeChanged e);
        void UpdateInvoice(InvoiceBusinessCustomerChanged e);
        void UpdateInvoice(InvoiceIssueChanged e);

        void UpdateInvoice(InvoiceStatusChanged e);
        void UpdateInvoice(InvoicePaidDateChanged e);


        void UpdateInvoice(InvoiceNumberChanged e);
        void UpdateInvoice(InvoicePaid e);
        void UpdateInvoice(InvoicePaymentFailed e);
        void UpdateInvoice(InvoiceSubmitted e);
        void UpdateInvoice(InvoiceReferenced e);
        void DeleteInvoice(InvoiceDeleted e);

        void InsertInvoiceItem(InvoiceItemAdded e);
        void UpdateInvoiceItem(InvoiceItemChanged e);
        void DeleteInvoiceItem(InvoiceItemRemoved e);

        void InsertProduct(TProduct product);
        void UpdateProduct<TProduct>(Guid product, params (Expression<Func<TProduct, object>> Property, object Value)[] updates);
        void UpdateProduct(TProduct product);
        void DeleteProduct(Guid product);

        void InsertOrder(TOrder order);
        void UpdateOrder(TOrder order);
        void DeleteOrder(Guid order);

        void InsertTax(TTax tax);
        void UpdateTax(TTax tax);
        void DeleteTax(Guid taxId);
    }
}
