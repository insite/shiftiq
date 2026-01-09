using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemPassPercentChanged : Change
    {
        public GradeItemPassPercentChanged(Guid item, decimal? passPercent)
        {
            Item = item;
            PassPercent = passPercent;
        }

        public Guid Item { get; set; }
        public decimal? PassPercent { get; set; }
    }
}
