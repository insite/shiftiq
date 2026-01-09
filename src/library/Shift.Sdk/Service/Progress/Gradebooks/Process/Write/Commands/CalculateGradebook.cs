using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks
{
    public class CalculateGradebook : Command
    {
        public Guid[] Learners { get; set; }

        public CalculateGradebook(Guid record, Guid[] learners)
        {
            AggregateIdentifier = record;
            Learners = learners;
        }
    }
}
