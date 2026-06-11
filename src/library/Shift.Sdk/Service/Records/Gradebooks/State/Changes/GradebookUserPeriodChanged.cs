using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookUserPeriodChanged : Change
    {
        public Guid User { get; }
        public Guid? Period { get; }

        public GradebookUserPeriodChanged(Guid user, Guid? period)
        {
            User = user;
            Period = period;
        }
    }
}
