using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Contacts
{
    public class MembershipAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new MembershipState();

        public MembershipState Data => (MembershipState)State;

        public void StartMembership(Guid user, Guid group, string function, DateTimeOffset effective)
        {
            Apply(new MembershipStarted(user, group, function, effective));
        }

        public void EndMembership()
        {
            Apply(new MembershipEnded());
        }

        public void ResumeMembership(Guid user, Guid group, string function, DateTimeOffset effective)
        {
            Apply(new MembershipResumed(user, group, function, effective));
        }

        public void ModifyMembershipEffective(DateTimeOffset effective)
        {
            Apply(new MembershipEffectiveModified(effective));
        }

        public void ModifyMembershipFunction(string function)
        {
            Apply(new MembershipFunctionModified(function));
        }

        public void ModifyMembershipExpiry(DateTimeOffset? expiry)
        {
            Apply(new MembershipExpiryModified(expiry));
        }

        public void ExpireMembership(DateTimeOffset expiry)
        {
            Apply(new MembershipExpired(expiry));
        }

        public void AddMembershipReason(Guid reasonIdentifier, string type, string subtype, DateTimeOffset effective, DateTimeOffset? expiry, string personOccupation)
        {
            var r = Data.Reasons.GetOrDefault(reasonIdentifier);
            if (r != null)
                return;

            Apply(new MembershipReasonAdded(reasonIdentifier, type, subtype, effective, expiry, personOccupation));
        }

        public void ModifyMembershipReason(Guid reasonIdentifier, string type, string subtype, DateTimeOffset effective, DateTimeOffset? expiry, string personOccupation)
        {
            var r = Data.Reasons.GetOrDefault(reasonIdentifier);
            if (r == null)
                return;

            var isChanged = r.Type != type
                || r.Subtype != subtype
                || r.Effective != effective
                || r.Expiry != expiry
                || r.PersonOccupation != personOccupation;

            if (!isChanged)
                return;

            Apply(new MembershipReasonModified(reasonIdentifier, type, subtype, effective, expiry, personOccupation));
        }

        public void RemoveMembershipReason(Guid reasonIdentifier)
        {
            var r = Data.Reasons.GetOrDefault(reasonIdentifier);
            if (r == null)
                return;

            Apply(new MembershipReasonRemoved(reasonIdentifier));
        }
    }
}