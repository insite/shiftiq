using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class RenameRubric : Command, IHasRun
    {
        public string RubricTitle { get; set; }

        public RenameRubric(Guid rubricId, string rubricTitle)
        {
            AggregateIdentifier = rubricId;
            RubricTitle = rubricTitle;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RubricTitle.IsEmpty())
                return true;

            var state = aggregate.Data;
            if (state.Content.Title.Text.Default == RubricTitle)
                return true;

            aggregate.Apply(new RubricRenamed(RubricTitle));

            return true;
        }
    }
}
