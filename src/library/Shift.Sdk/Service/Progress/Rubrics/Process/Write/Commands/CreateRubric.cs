using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class CreateRubric : Command, IHasRun, IHasAggregate
    {
        private RubricAggregate Aggregate { get; set; }

        RubricAggregate IHasAggregate.Aggregate => Aggregate;

        public string RubricTitle { get; set; }

        public CreateRubric(Guid rubricId, string rubricTitle)
        {
            AggregateIdentifier = rubricId;
            RubricTitle = rubricTitle;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (RubricTitle.IsEmpty())
                return false;

            var change = new RubricCreated(RubricTitle);
            change.Identify(OriginOrganization, OriginUser);

            Aggregate = new RubricAggregate { AggregateIdentifier = AggregateIdentifier };
            Aggregate.Apply(change);

            return true;
        }
    }
}
