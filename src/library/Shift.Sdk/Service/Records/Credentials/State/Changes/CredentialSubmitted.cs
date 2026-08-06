using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CredentialSubmitted : Change
    {
        public CredentialSubmitted(DateTimeOffset submitted, string description)
        {
            Submitted = submitted;
            Description = description;
        }

        public DateTimeOffset Submitted { get; set; }
        public string Description { get; set; }
    }
}
