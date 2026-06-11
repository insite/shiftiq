using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class DescribeRubric : Command, IHasRun
    {
        public string RubricDescription { get; set; }

        public DescribeRubric(Guid rubricId, string rubricDescription)
        {
            AggregateIdentifier = rubricId;
            RubricDescription = rubricDescription;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RubricDescription.IsEmpty())
                RubricDescription = null;

            var state = aggregate.Data;
            if (state.Content.Description.Text.Default == RubricDescription)
                return true;

            aggregate.Apply(new RubricDescribed(RubricDescription));

            return true;
        }
    }
}
