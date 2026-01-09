using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class ChangeCredentialAuthority : Command
    {
        public ChangeCredentialAuthority(Guid credential, Guid? authorityIdentifier, string authorityName, string authorityType, string location, string reference, decimal? hours)
        {
            AggregateIdentifier = credential;
            AuthorityIdentifier = authorityIdentifier;
            AuthorityName = authorityName;
            AuthorityType = authorityType;
            Location = location;
            Reference = reference;
            Hours = hours;
        }

        public Guid? AuthorityIdentifier { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityType { get; set; }

        public string Location { get; set; }
        public string Reference { get; set; }

        public decimal? Hours { get; set; }
    }
}