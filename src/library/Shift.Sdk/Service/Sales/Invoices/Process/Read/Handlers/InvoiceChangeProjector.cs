using Shift.Common.Timeline.Changes;

using InSite.Domain.Invoices;
using InSite.Domain.Sales.Invoices.Changes;

namespace InSite.Application.Invoices.Read
{
    /// <summary>
    /// Implements the projector for Invoice events.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class InvoiceChangeProjector
    {
        private readonly IInvoiceStore _store;

        public InvoiceChangeProjector(IChangeQueue publisher, IInvoiceStore store)
        {
            _store = store;

            publisher.Subscribe<InvoiceCustomerChanged>(Handle);
            publisher.Subscribe<InvoiceStatusChanged>(Handle);
            publisher.Subscribe<InvoicePaidDateChanged>(Handle);
            publisher.Subscribe<InvoiceDrafted>(Handle);
            publisher.Subscribe<InvoiceItemAdded>(Handle);
            publisher.Subscribe<InvoiceItemChanged>(Handle);
            publisher.Subscribe<InvoiceItemRemoved>(Handle);
            publisher.Subscribe<InvoiceNumberChanged>(Handle);
            publisher.Subscribe<InvoiceSubmitted>(Handle);
            publisher.Subscribe<InvoicePaid>(Handle);
            publisher.Subscribe<InvoicePaymentFailed>(Handle);
            publisher.Subscribe<InvoiceDeleted>(Handle);
            publisher.Subscribe<InvoiceReferenced>(Handle);
            publisher.Subscribe<InvoiceIssueChanged>(Handle);
            publisher.Subscribe<InvoiceBusinessCustomerChanged>(Handle);
            publisher.Subscribe<InvoiceEmployeeChanged>(Handle);
        }

        public void Handle(InvoiceCustomerChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceBusinessCustomerChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceEmployeeChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceIssueChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceStatusChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoicePaidDateChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceDrafted e)
        {
            _store.InsertInvoice(e);
        }

        public void Handle(InvoiceItemAdded e)
        {
            _store.InsertInvoiceItem(e);
        }

        public void Handle(InvoiceItemChanged e)
        {
            _store.UpdateInvoiceItem(e);
        }

        public void Handle(InvoiceItemRemoved e)
        {
            _store.DeleteInvoiceItem(e);
        }

        public void Handle(InvoiceNumberChanged e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceSubmitted e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoicePaid e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoicePaymentFailed e)
        {
            _store.UpdateInvoice(e);
        }

        public void Handle(InvoiceDeleted e)
        {
            _store.DeleteInvoice(e);
        }

        public void Handle(InvoiceReferenced e)
        {
            _store.UpdateInvoice(e);
        }
    }
}
