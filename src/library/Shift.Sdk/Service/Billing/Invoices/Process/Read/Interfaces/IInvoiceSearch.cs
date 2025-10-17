using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Invoices.Read
{
    public interface IInvoiceSearch
    {
        bool InvoiceHasRegistration(Guid invoiceIdentifier);
        bool InvoiceIsPaid(Guid invoiceIdentifier);
        VInvoice GetInvoice(Guid invoice, params Expression<Func<VInvoice, object>>[] includes);
        List<VInvoice> GetInvoices();
        int CountInvoices(VInvoiceFilter filter);
        List<VInvoice> GetInvoices(VInvoiceFilter filter, params Expression<Func<VInvoice, object>>[] includes);

        List<QInvoiceItem> GetInvoiceItems();
        int CountInvoiceItems(Guid invoiceIdentifiier);
        List<QInvoiceItem> GetInvoiceItems(Guid invoiceIdentifier, params Expression<Func<QInvoiceItem, object>>[] includes);
        QInvoiceItem GetInvoiceItem(Guid invoiceIdentifier, Guid itemIdentifier);
        decimal GetInvoiceTotalAmount(Guid invoiceIdentifier);

        TProduct GetProduct(Guid product);
        int CountProducts(TProductFilter filter);
        List<TProduct> GetProducts(TProductFilter filter);

        int CountOrders(TOrderFilter filter);
        List<TOrder> GetOrders(TOrderFilter filter, params Expression<Func<TOrder, object>>[] includes);

        TTax GetTax(Guid taxId);
        int CountTaxes(TTaxFilter filter);
        List<TTax> GetTaxes(TTaxFilter filter);
    }
}
