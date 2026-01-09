using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeCandidate : Command
    {
        public Guid Candidate { get; set; }

        public ChangeCandidate(Guid aggregate, Guid candidate)
        {
            AggregateIdentifier = aggregate;
            Candidate = candidate;
        }
    }
}
