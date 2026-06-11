using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RemoveComment : Command
    {
        public Guid CommentIdentifier { get; set; }

        public RemoveComment(Guid id, Guid comment)
        {
            AggregateIdentifier = id;
            CommentIdentifier = comment;
        }
    }
}
