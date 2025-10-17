using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookPeriodChanged : Change
    {
        public Guid? Period { get; }

        public GradebookPeriodChanged(Guid? period)
        {
            Period = period;
        }
    }
}
