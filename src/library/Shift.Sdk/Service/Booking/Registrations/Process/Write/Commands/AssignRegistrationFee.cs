using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignRegistrationFee : Command
    {
        public decimal? Fee { get; set; }

        public AssignRegistrationFee(Guid aggregate, decimal? fee)
        {
            AggregateIdentifier = aggregate;
            Fee = fee;
        }
    }
}
