using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteCriterion : Command
    {
        public Guid Criterion { get; set; }

        public DeleteCriterion(Guid bank, Guid criterion)
        {
            AggregateIdentifier = bank;
            Criterion = criterion;
        }
    }
}
