using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class DeleteGradebook : Command
    {

        public DeleteGradebook(Guid record)
        {
            AggregateIdentifier = record;
        }
    }
}