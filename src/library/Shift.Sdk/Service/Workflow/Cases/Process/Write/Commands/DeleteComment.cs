using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class DeleteComment : Command
    {
        public Guid Comment { get; set; }

        public DeleteComment(Guid aggregate, Guid comment)
        {
            AggregateIdentifier = aggregate;
            Comment = comment;
        }
    }
}
