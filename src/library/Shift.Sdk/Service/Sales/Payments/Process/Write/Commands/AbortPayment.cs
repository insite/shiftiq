using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Payments;

namespace InSite.Application.Gateways.Write
{
    public class AbortPayment : Command
    {
        public AbortPayment(Guid gateway, Guid payment, ErrorResponse response)
        {
            AggregateIdentifier = gateway;
            Payment = payment;
            Response = response;
        }

        public Guid Payment { get; set; }
        public ErrorResponse Response { get; }
    }
}