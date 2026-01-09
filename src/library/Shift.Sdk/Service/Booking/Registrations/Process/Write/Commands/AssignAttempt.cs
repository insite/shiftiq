using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignAttempt : Command
    {
        public Guid Attempt { get; set; }

        public AssignAttempt(Guid aggregate, Guid attempt)
        {
            AggregateIdentifier = aggregate;
            Attempt = attempt;
        }
    }
}
