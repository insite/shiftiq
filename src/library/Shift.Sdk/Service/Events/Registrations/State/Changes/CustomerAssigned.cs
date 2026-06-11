using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class CustomerAssigned : Change
    {
        public Guid? Customer { get; set; }

        public CustomerAssigned(Guid? customer)
        {
            Customer = customer;
        }
    }
}
