using System;

namespace InSite.Persistence
{
    public class TApplication
    {
        public Guid ApplicationIdentifier { get; set; }
        public Guid CandidateUserIdentifier { get; set; }
        public Guid OpportunityIdentifier { get; set; }

        public string CandidateLetter { get; set; }
        public string CandidateResume { get; set; }

        public DateTimeOffset WhenCreated { get; set; }
        public DateTimeOffset WhenModified { get; set; }

        public virtual User CandidateUser { get; set; }
        public virtual TOpportunity Opportunity { get; set; }
    }
}
