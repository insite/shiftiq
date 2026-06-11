using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Progresses.Write
{
    public class ChangeProgressComment : Command
    {
        public ChangeProgressComment(Guid progress, string comment)
        {
            AggregateIdentifier = progress;
            Comment = comment;
        }

        public string Comment { get; set; }
    }
}
