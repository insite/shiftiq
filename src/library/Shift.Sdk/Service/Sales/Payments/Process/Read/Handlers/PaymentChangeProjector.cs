using Shift.Common.Timeline.Changes;

using InSite.Domain.Payments;

namespace InSite.Application.Payments.Read
{
    /// <summary>
    /// Implements the projector for Event changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Events can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from changes to the projection tables). A processor,
    /// in contrast, should *never* replay past events.
    /// </remarks>
    public class PaymentChangeProjector
    {
        private readonly IPaymentStore _store;

        public PaymentChangeProjector(IChangeQueue publisher, IPaymentStore store)
        {
            _store = store;

            publisher.Subscribe<GatewayOpened>(Handle);
            publisher.Subscribe<GatewayClosed>(Handle);

            publisher.Subscribe<MerchantAdded>(Handle);
            publisher.Subscribe<MerchantRemoved>(Handle);

            publisher.Subscribe<MerchantActivated>(Handle);
            publisher.Subscribe<MerchantDeactivated>(Handle);

            publisher.Subscribe<PaymentImported>(Handle);
            publisher.Subscribe<PaymentStarted>(Handle);
            publisher.Subscribe<PaymentAborted>(Handle);
            publisher.Subscribe<PaymentCompleted>(Handle);
            publisher.Subscribe<PaymentCreatedByModified>(Handle);
        }

        public void Handle(GatewayOpened e) { }
        public void Handle(GatewayClosed e) { }

        public void Handle(MerchantAdded e) { }
        public void Handle(MerchantRemoved e) { }

        public void Handle(MerchantActivated e) { }
        public void Handle(MerchantDeactivated e) { }

        public void Handle(PaymentImported e) => _store.InsertPayment(e);
        public void Handle(PaymentStarted e) => _store.InsertPayment(e);
        public void Handle(PaymentAborted e) => _store.UpdatePayment(e);
        public void Handle(PaymentCompleted e) => _store.UpdatePayment(e);
        public void Handle(PaymentCreatedByModified e) => _store.UpdatePayment(e);
    }
}
