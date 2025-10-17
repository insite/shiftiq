using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class SingleQuestionPerTabDisabled : Change
    {
        public Guid Specification { get; set; }

        public SingleQuestionPerTabDisabled(Guid specification)
        {
            Specification = specification;
        }
    }
}
