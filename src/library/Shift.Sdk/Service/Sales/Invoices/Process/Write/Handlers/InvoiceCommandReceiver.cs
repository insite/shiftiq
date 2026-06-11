using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Sales.Invoices.Write.Commands;
using InSite.Domain.Invoices;

namespace InSite.Application.Invoices.Write
{
    public class InvoiceCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public InvoiceCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddInvoiceItem>(Handle);
            commander.Subscribe<ChangeInvoiceCustomer>(Handle);
            commander.Subscribe<ChangeInvoiceStatus>(Handle);
            commander.Subscribe<ChangeInvoicePaidDate>(Handle);
            commander.Subscribe<ChangeInvoiceNumber>(Handle);
            commander.Subscribe<ChangeInvoiceItem>(Handle);
            commander.Subscribe<DraftInvoice>(Handle);
            commander.Subscribe<ChangeBusinessCustomer>(Handle);
            commander.Subscribe<ChangeEmployee>(Handle);
            commander.Subscribe<ChangeIssue>(Handle);
            commander.Subscribe<RemoveInvoiceItem>(Handle);
            commander.Subscribe<SubmitInvoice>(Handle);
            commander.Subscribe<PayInvoice>(Handle);
            commander.Subscribe<FailInvoicePayment>(Handle);
            commander.Subscribe<DeleteInvoice>(Handle);
            commander.Subscribe<ReferenceInvoice>(Handle);
        }

        private void Commit(InvoiceAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(AddInvoiceItem c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.AddInvoiceItem(c.Item);
            Commit(aggregate, c);
        }

        public void Handle(ChangeInvoiceCustomer c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceCustomer(c.Customer);
            Commit(aggregate, c);
        }

        public void Handle(ChangeInvoiceStatus c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceStatus(c.InvoiceStatus);
            Commit(aggregate, c);
        }

        public void Handle(ChangeInvoicePaidDate c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoicePaidDate(c.InvocePaidDate);
            Commit(aggregate, c);
        }

        public void Handle(ChangeInvoiceNumber c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceNumber(c.Number);
            Commit(aggregate, c);
        }

        public void Handle(ChangeInvoiceItem c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceItem(c.Item);
            Commit(aggregate, c);
        }

        public void Handle(DraftInvoice c)
        {
            var aggregate = new InvoiceAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.DraftInvoice(c.Tenant, c.Number, c.Customer, c.Items);
            Commit(aggregate, c);
        }

        public void Handle(ChangeBusinessCustomer c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceBusinessCustomer(c.BusinessCustomer);
            Commit(aggregate, c);
        }

        public void Handle(ChangeEmployee c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceEmployee(c.Employee);
            Commit(aggregate, c);
        }

        public void Handle(ChangeIssue c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ChangeInvoiceIssue(c.Issue);
            Commit(aggregate, c);
        }

        public void Handle(RemoveInvoiceItem c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.RemoveInvoiceItem(c.ItemIdentifier);
            Commit(aggregate, c);
        }

        public void Handle(SubmitInvoice c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.SubmitInvoice();
            Commit(aggregate, c);
        }

        public void Handle(PayInvoice c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.PayInvoice(c.Paid);
            Commit(aggregate, c);
        }

        public void Handle(FailInvoicePayment c)
        {
            _repository.LockAndRun<InvoiceAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.FailInvoicePayment();
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteInvoice c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.DeleteInvoice();
            Commit(aggregate, c);
        }

        public void Handle(ReferenceInvoice c)
        {
            var aggregate = _repository.Get<InvoiceAggregate>(c.AggregateIdentifier);
            aggregate.ReferenceInvoice(c.ReferencedInvoice);
            Commit(aggregate, c);
        }
    }
}