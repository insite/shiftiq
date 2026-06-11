using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class CommentRegistration : Command
    {
        public string Comment { get; set; }

        public CommentRegistration(Guid aggregate, string comment)
        {
            AggregateIdentifier = aggregate;
            Comment = comment;
        }
    }
}
