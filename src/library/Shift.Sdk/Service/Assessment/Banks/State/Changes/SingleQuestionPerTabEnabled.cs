using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SingleQuestionPerTabEnabled : Change
    {
        public Guid Specification { get; set; }

        public SingleQuestionPerTabEnabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
