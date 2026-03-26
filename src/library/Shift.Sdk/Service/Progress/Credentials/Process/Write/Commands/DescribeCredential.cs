using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class DescribeCredential : Command
    {
        public string Description { get; set; }

        public DescribeCredential(Guid credential, string description)
        {
            AggregateIdentifier = credential;
            Description = description;
        }
    }
}
