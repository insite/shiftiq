using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Payments
{
    public class GatewayAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Gateway();

        public Gateway Data => (Gateway)State;

        public void OpenGateway(Guid organization, GatewayType type, string name)
        {
            Apply(new GatewayOpened(organization, type, name));
        }

        public void AddMerchant(Guid organization, string merchant, string description)
        {

        }

        public void ActivateMerchant(Guid organization, EnvironmentName environment, string token)
        {

        }

        public void ImportPayment(
            Guid organization,
            Guid invoice,
            Guid payment,
            Guid createdBy,
            string orderNumber,
            PaymentStatus status,
            DateTimeOffset? started,
            DateTimeOffset? approved,
            decimal amount,
            string customerIP,
            string transactionId,
            string resultCode,
            string resultMessage
            )
        {
            Apply(new PaymentImported(
                organization,
                invoice,
                payment,
                createdBy,
                orderNumber,
                status,
                started,
                approved,
                amount,
                customerIP,
                transactionId,
                resultCode,
                resultMessage
            ));
        }

        public void StartPayment(Guid organization, Guid invoice, Guid payment, PaymentRequest request)
        {
            Apply(new PaymentStarted(organization, invoice, payment, request));
        }

        public void AbortPayment(Guid payment, ErrorResponse response)
        {
            if (!Data.PaymentExists(payment))
                throw new PaymentNotFoundException(payment);

            Apply(new PaymentAborted(payment, response));
        }

        public void CompletePayment(Guid payment, PaymentResponse response)
        {
            if (!Data.PaymentExists(payment))
                throw new PaymentNotFoundException(payment);

            Apply(new PaymentCompleted(payment, response));
        }

        public void ModifyPaymentCreatedBy(Guid payment, Guid createdBy)
        {
            if (!Data.PaymentExists(payment))
                throw new PaymentNotFoundException(payment);

            Apply(new PaymentCreatedByModified(payment, createdBy));
        }

        public void DeactivateMerchant(Guid organization)
        {
            Apply(new MerchantDeactivated(organization));
        }

        public void RemoveMerchant(Guid organization)
        {
            Apply(new MerchantRemoved(organization));
        }

        public void CloseGateway()
        {
            Apply(new GatewayClosed());
        }
    }
}
