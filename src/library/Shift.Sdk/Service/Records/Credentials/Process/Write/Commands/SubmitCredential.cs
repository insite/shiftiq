using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class SubmitCredential : Command
    {
        public SubmitCredential(Guid credential, DateTimeOffset submitted, string description)
        {
            AggregateIdentifier = credential;
            Submitted = submitted;
            Description = description;
        }

        public DateTimeOffset Submitted { get; private set; }
        public string Description { get; private set; }
    }
}
