using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SpecificationReconfigured : Change
    {
        public Guid Specification { get; set; }
        public ConsequenceType? Consequence { get; set; }
        public int FormLimit { get; set; }
        public int QuestionLimit { get; set; }

        public SpecificationReconfigured(Guid specification, ConsequenceType? consequence, int formLimit, int questionLimit)
        {
            Specification = specification;
            Consequence = consequence;
            FormLimit = formLimit;
            QuestionLimit = questionLimit;
        }
    }
}
