using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class UnlockGradebook : Command
    {
        public UnlockGradebook(Guid record)
        {
            AggregateIdentifier = record;
        }
    }
}
