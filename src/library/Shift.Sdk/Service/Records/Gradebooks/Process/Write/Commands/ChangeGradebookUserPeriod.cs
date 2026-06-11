using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradebookUserPeriod : Command
    {
        public Guid User { get; }
        public Guid? Period { get; }

        public ChangeGradebookUserPeriod(Guid gradebook, Guid user, Guid? period)
        {
            AggregateIdentifier = gradebook;
            User = user;
            Period = period;
        }
    }
}
