using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Activities.Write
{
    public class IncreaseCandidateLimit : Command
    {
        public int Increase { get; set; }

        public IncreaseCandidateLimit(Guid activity, int increase)
        {
            AggregateIdentifier = activity;
            Increase = increase;
        }
    }
}
