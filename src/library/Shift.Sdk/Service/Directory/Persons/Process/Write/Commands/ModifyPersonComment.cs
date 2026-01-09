using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.People.Write
{
    public class ModifyPersonComment : Command
    {
        public CommentActionType CommentActionType { get; set; }
        public PersonComment Comment { get; set; }

        public ModifyPersonComment(Guid personId, CommentActionType commentActionType, PersonComment comment)
        {
            AggregateIdentifier = personId;
            CommentActionType = commentActionType;
            Comment = comment;
        }
    }
}
