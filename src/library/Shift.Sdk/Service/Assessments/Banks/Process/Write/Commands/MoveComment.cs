using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class MoveComment : Command
    {
        public Guid Comment { get; set; }
        public CommentType Type { get; set; }
        public Guid Subject { get; set; }

        public MoveComment(Guid bank, Guid comment, CommentType type, Guid subject)
        {
            AggregateIdentifier = bank;
            Comment = comment;
            Type = type;
            Subject = subject;
        }
    }
}
