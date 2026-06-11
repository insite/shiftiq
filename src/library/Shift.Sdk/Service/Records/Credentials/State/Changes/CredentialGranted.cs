using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class CredentialGranted : Change
    {
        public CredentialGranted(Guid user, DateTimeOffset granted, string authority, string location, string reference, decimal? hours)
        {
            User = user;
            Granted = granted;


            Authority = authority;
            Location = location;
            Reference = reference;
            Hours = hours;
        }

        public Guid User { get; set; }
        public DateTimeOffset Granted { get; set; }

        public string Authority { get; set; }
        public string Location { get; set; }
        public string Reference { get; set; }
        public decimal? Hours { get; set; }
    }

    [Obsolete]
    public class CredentialGranted2 : Change
    {
        public CredentialGranted2(Guid user, DateTimeOffset granted,
            Guid? authorityIdentifier = null, string authorityName = null, string authorityType = null,
            string location = null, string reference = null,
            decimal? hours = null)
        {
            User = user;
            Granted = granted;

            AuthorityIdentifier = authorityIdentifier;
            AuthorityName = authorityName;
            AuthorityType = authorityType;

            Location = location;
            Reference = reference;
            Hours = hours;
        }

        public Guid User { get; set; }
        public DateTimeOffset Granted { get; set; }

        public Guid? AuthorityIdentifier { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityType { get; set; }

        public string Location { get; set; }
        public string Reference { get; set; }

        public decimal? Hours { get; set; }
    }

    public class CredentialGranted3 : Change
    {
        public CredentialGranted3(DateTimeOffset granted, string description, decimal? score)
        {
            Granted = granted;
            Description = description;
            Score = score;
        }

        public DateTimeOffset Granted { get; set; }
        public string Description { get; set; }
        public decimal? Score { get; set; }
    }

    public class CredentialAuthorityChanged : Change
    {
        public CredentialAuthorityChanged(Guid? authorityIdentifier, string authorityName, string authorityType, string location, string reference, decimal? hours)
        {
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

    public class CredentialExpirationChanged : Change
    {
        public CredentialExpirationChanged(Expiration expiration)
        {
            Expiration = expiration;
        }

        public Expiration Expiration { get; set; }
    }

    public class CredentialTagged : Change
    {
        public CredentialTagged(string necessity, string priority)
        {
            Necessity = necessity;
            Priority = priority;
        }

        public string Necessity { get; set; }
        public string Priority { get; set; }
    }

    public class CredentialPublishedOnChain : Change
    {
        public string PublisherAddress { get; set; }
        public string TransactionHash { get; set; }
        public int Status { get; set; }
        public CredentialPublishedOnChain(int status,string transactionHash, string publisherAddress)
        {
            TransactionHash = transactionHash;
            PublisherAddress = publisherAddress;
            Status = status;
        }
    }
}