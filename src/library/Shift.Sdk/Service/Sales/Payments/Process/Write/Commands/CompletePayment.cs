using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Payments;

namespace InSite.Application.Gateways.Write
{
    public class CompletePayment : Command
    {
        public CompletePayment(Guid gateway, Guid payment, PaymentResponse response)
        {
            AggregateIdentifier = gateway;
            Payment = payment;
            Response = response;
        }

        public Guid Payment { get; set; }
        public PaymentResponse Response { get; }
    }
}