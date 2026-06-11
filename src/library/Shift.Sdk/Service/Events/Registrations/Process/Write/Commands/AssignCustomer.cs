using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignCustomer : Command
    {
        public Guid? Customer { get; set; }

        public AssignCustomer(Guid aggregate, Guid? customer)
        {
            AggregateIdentifier = aggregate;
            Customer = customer;
        }
    }
}
