using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookCalculated : Change
    {
        public GradebookCalculated(Guid[] learners)
        {
            Learners = learners;
        }

        public Guid[] Learners { get; set; }
    }
}
