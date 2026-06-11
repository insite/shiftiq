using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class CompleteEventPublication : Command
    {
        public string Status { get; set; }
        public string Errors { get; set; }

        public CompleteEventPublication(Guid aggregate, string status, string errors)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Errors = errors;
        }
    }
}
