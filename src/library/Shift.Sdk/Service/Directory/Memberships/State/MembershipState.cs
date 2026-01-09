using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    [Serializable]
    public class MembershipState : AggregateState
    {
        public Guid Group { get; set; }
        public Guid User { get; set; }
        public Guid Organization { get; set; }

        public string Function { get; set; }

        public DateTimeOffset Effective { get; set; }
        public DateTimeOffset? Expiry { get; set; }

        public Dictionary<Guid, MembershipReason> Reasons { get; set; }

        public void When(MembershipStarted e)
        {
            Group = e.Group;
            User = e.User;
            Organization = e.OriginOrganization;

            Function = e.Function;
            Effective = e.Effective;

            Reasons = new Dictionary<Guid, MembershipReason>();
        }

        public void When(MembershipStopped e)
        {

        }

        public void When(MembershipEnded e)
        {

        }

        public void When(MembershipResumed e)
        {
            Group = e.Group;
            User = e.User;
            Organization = e.OriginOrganization;

            Function = e.Function;
            Effective = e.Effective;
        }

        public void When(MembershipFunctionModified e)
        {
            Function = e.Function;
        }

        public void When(MembershipEffectiveModified e)
        {
            Effective = e.Effective;
        }

        public void When(MembershipExpiryModified e)
        {
            Expiry = e.Expiry;
        }

        public void When(MembershipExpired e)
        {
            Expiry = e.Expiry;
        }

        public void When(MembershipReasonAdded e)
        {
            var reason = new MembershipReason
            {
                Identifier = e.ReasonIdentifier,
                Type = e.Type,
                Subtype = e.Subtype,
                Effective = e.Effective,
                Expiry = e.Expiry,
                PersonOccupation = e.PersonOccupation
            };

            Reasons.Add(e.ReasonIdentifier, reason);
        }

        public void When(MembershipReasonModified e)
        {
            var reason = Reasons[e.ReasonIdentifier];
            reason.Type = e.Type;
            reason.Subtype = e.Subtype;
            reason.Effective = e.Effective;
            reason.Expiry = e.Expiry;
            reason.PersonOccupation = e.PersonOccupation;
        }

        public void When(MembershipReasonRemoved e)
        {
            Reasons.Remove(e.ReasonIdentifier);
        }
    }
}