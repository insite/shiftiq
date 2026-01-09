using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Payments;

namespace InSite.Application.Gateways.Write
{
    public class StartPayment : Command
    {
        public StartPayment(Guid gateway, Guid organization, Guid invoice, Guid payment, PaymentInput input)
        {
            AggregateIdentifier = gateway;
            Organization = organization;
            Invoice = invoice;
            Payment = payment;
            Input = input;
        }

        public Guid Organization { get; set; }
        public Guid Invoice { get; set; }
        public Guid Payment { get; set; }
        public PaymentInput Input { get; set; }
    }
}