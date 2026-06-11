using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ReconfigureSpecification : Command
    {
        public Guid Specification { get; set; }
        public ConsequenceType? Consequence { get; set; }
        public int FormLimit { get; set; }
        public int QuestionLimit { get; set; }

        public ReconfigureSpecification(Guid bank, Guid specification, ConsequenceType? consequence, int formLimit, int questionLimit)
        {
            AggregateIdentifier = bank;
            Specification = specification;
            FormLimit = formLimit;
            QuestionLimit = questionLimit;
            Consequence = consequence;
        }
    }
}