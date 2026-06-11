using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradebookPeriod : Command
    {
        public Guid? Period { get; }

        public ChangeGradebookPeriod(Guid gradebook, Guid? period)
        {
            AggregateIdentifier = gradebook;
            Period = period;
        }
    }
}
