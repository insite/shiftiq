using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    public class DeleteRubric : Command, IHasRun
    {
        public DeleteRubric(Guid rubricId)
        {
            AggregateIdentifier = rubricId;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.IsDeleted)
                return false;

            aggregate.Apply(new RubricDeleted());

            return true;
        }
    }
}
