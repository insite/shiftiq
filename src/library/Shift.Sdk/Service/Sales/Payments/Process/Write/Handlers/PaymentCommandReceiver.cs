using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Gateways.Write;
using InSite.Domain.Payments;

using Shift.Constant;

namespace InSite.Application.Payments.Write
{
    public class PaymentCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public PaymentCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<OpenGateway>(Handle);
            commander.Subscribe<CloseGateway>(Handle);

            commander.Subscribe<AddMerchant>(Handle);
            commander.Subscribe<RemoveMerchant>(Handle);

            commander.Subscribe<ActivateMerchant>(Handle);
            commander.Subscribe<DeactivateMerchant>(Handle);

            commander.Subscribe<ImportPayment>(Handle);
            commander.Subscribe<StartPayment>(Handle);
            commander.Subscribe<AbortPayment>(Handle);
            commander.Subscribe<CompletePayment>(Handle);
            commander.Subscribe<ModifyPaymentCreatedBy>(Handle);
        }

        private void Commit(GatewayAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        /// <summary>
        /// Opens a new payment gateway. If the gateway already exists and its status is Closed then this function 
        /// reopens the the gateway.
        /// </summary>
        public void Handle(OpenGateway c)
        {
            var aggregate = _repository.Exists<GatewayAggregate>(c.AggregateIdentifier)
                ? _repository.Get<GatewayAggregate>(c.AggregateIdentifier)
                : new GatewayAggregate { AggregateIdentifier = c.AggregateIdentifier };

            if (aggregate.Data?.Status == GatewayStatus.Open)
                return;

            aggregate.OpenGateway(c.Tenant, c.Type, c.Name);
            Commit(aggregate, c);
        }

        public void Handle(CloseGateway c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.CloseGateway();
            Commit(aggregate, c);
        }

        public void Handle(AddMerchant c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.AddMerchant(c.Tenant, c.Merchant, c.Description);
            Commit(aggregate, c);
        }

        public void Handle(RemoveMerchant c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.RemoveMerchant(c.Tenant);
            Commit(aggregate, c);
        }

        public void Handle(ActivateMerchant c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.ActivateMerchant(c.Tenant, c.Environment, c.Token);
            Commit(aggregate, c);
        }

        public void Handle(DeactivateMerchant c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.DeactivateMerchant(c.Tenant);
            Commit(aggregate, c);
        }

        public void Handle(ImportPayment c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.ImportPayment(
                c.Tenant,
                c.Invoice,
                c.Payment,
                c.CreatedBy,
                c.OrderNumber,
                c.Status,
                c.Started,
                c.Approved,
                c.Amount,
                c.CustomerIP,
                c.TransactionId,
                c.ResultCode,
                c.ResultMessage
            );
            Commit(aggregate, c);
        }

        public void Handle(StartPayment c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.StartPayment(c.Organization, c.Invoice, c.Payment, c.Input.CreateRequest());
            Commit(aggregate, c);
        }

        public void Handle(AbortPayment c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.AbortPayment(c.Payment, c.Response);
            Commit(aggregate, c);
        }

        public void Handle(CompletePayment c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.CompletePayment(c.Payment, c.Response);
            Commit(aggregate, c);
        }

        public void Handle(ModifyPaymentCreatedBy c)
        {
            var aggregate = _repository.Get<GatewayAggregate>(c.AggregateIdentifier);
            aggregate.ModifyPaymentCreatedBy(c.Payment, c.CreatedBy);
            Commit(aggregate, c);
        }
    }
}
