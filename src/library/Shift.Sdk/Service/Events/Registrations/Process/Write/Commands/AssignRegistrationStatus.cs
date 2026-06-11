using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignRegistrationStatus : Command
    {
        public string Status { get; set; }

        public AssignRegistrationStatus(Guid aggregate, string status)
        {
            AggregateIdentifier = aggregate;
            Status = status;
        }
    }
}
