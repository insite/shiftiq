using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gateways.Write
{
    public class ModifyPaymentCreatedBy : Command
    {
        public Guid Payment { get; set; }
        public Guid CreatedBy { get; set; }

        public ModifyPaymentCreatedBy(Guid gateway, Guid payment, Guid createdBy)
        {
            AggregateIdentifier = gateway;
            Payment = payment;
            CreatedBy = createdBy;
        }
    }
}
