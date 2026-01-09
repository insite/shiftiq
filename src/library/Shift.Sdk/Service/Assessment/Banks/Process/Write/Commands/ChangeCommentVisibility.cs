using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeCommentVisibility : Command
    {
        public Guid Comment { get; set; }
        public bool IsHidden { get; set; }

        public ChangeCommentVisibility(Guid bank, Guid comment, bool isHidden)
        {
            AggregateIdentifier = bank;
            Comment = comment;
            IsHidden = isHidden;
        }
    }
}
