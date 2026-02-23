using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class RejectComment : Command
    {
        public Guid Comment { get; set; }

        public RejectComment(Guid bank, Guid comment)
        {
            AggregateIdentifier = bank;
            Comment = comment;
        }
    }

    public class RetractComment : Command
    {
        public Guid Comment { get; set; }

        public RetractComment(Guid bank, Guid comment)
        {
            AggregateIdentifier = bank;
            Comment = comment;
        }
    }
}