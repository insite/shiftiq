using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class DeleteComment : Command
    {
        public Guid Comment { get; }

        public DeleteComment(Guid journal, Guid comment)
        {
            AggregateIdentifier = journal;
            Comment = comment;
        }
    }
}
