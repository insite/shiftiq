using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class LockGradebook : Command
    {
        public LockGradebook(Guid record)
        {
            AggregateIdentifier = record;
        }
    }
}
