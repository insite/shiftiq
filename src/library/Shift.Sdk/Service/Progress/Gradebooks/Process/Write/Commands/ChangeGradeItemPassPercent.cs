using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemPassPercent : Command
    {
        public ChangeGradeItemPassPercent(Guid record, Guid item, decimal? passPercent)
        {
            AggregateIdentifier = record;
            Item = item;
            PassPercent = passPercent;
        }

        public Guid Item { get; set; }
        public decimal? PassPercent { get; set; }
    }
}
