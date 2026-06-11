using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class DeleteEnrollment : Command
    {
        public DeleteEnrollment(Guid gradebook, Guid learner)
        {
            AggregateIdentifier = gradebook;
            Learner = learner;
        }

        public Guid Learner { get; set; }
    }
}