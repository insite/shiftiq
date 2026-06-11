using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class AddProgress : Command
    {
        public AddProgress(Guid progress, Guid gradebook, Guid gradeitem, Guid learner)
        {
            AggregateIdentifier = progress;
            Gradebook = gradebook;
            GradeItem = gradeitem;
            Learner = learner;
        }

        public Guid Gradebook { get; set; }
        public Guid GradeItem { get; set; }
        public Guid Learner { get; set; }
    }
}
