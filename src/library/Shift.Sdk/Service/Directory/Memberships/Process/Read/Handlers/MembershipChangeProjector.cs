using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the projector for Membership changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class MembershipChangeProjector
    {
        private readonly IMembershipStore _membershipStore;
        private readonly IMembershipReasonStore _reasonStore;

        public MembershipChangeProjector(IChangeQueue publisher, IMembershipStore membershipStore, IMembershipReasonStore reasonStore)
        {
            _membershipStore = membershipStore;
            _reasonStore = reasonStore;

            publisher.Subscribe<MembershipStarted>(Handle);
            publisher.Subscribe<MembershipStopped>(Handle);
            publisher.Subscribe<MembershipEnded>(Handle);
            publisher.Subscribe<MembershipResumed>(Handle);
            publisher.Subscribe<MembershipEffectiveModified>(Handle);
            publisher.Subscribe<MembershipFunctionModified>(Handle);

            publisher.Subscribe<MembershipExpiryModified>(Handle);
            publisher.Subscribe<MembershipExpired>(Handle);

            publisher.Subscribe<MembershipReasonAdded>(Handle);
            publisher.Subscribe<MembershipReasonModified>(Handle);
            publisher.Subscribe<MembershipReasonRemoved>(Handle);
        }

        public void Handle(MembershipEffectiveModified e)
            => _membershipStore.Update(e, x => x.MembershipEffective = e.Effective);

        public void Handle(MembershipEnded e)
            => _membershipStore.Delete(e.AggregateIdentifier);

        public void Handle(MembershipExpired e)
            => _membershipStore.Delete(e.AggregateIdentifier);

        public void Handle(MembershipExpiryModified e)
            => _membershipStore.Update(e, x => x.MembershipExpiry = e.Expiry);

        public void Handle(MembershipFunctionModified e)
            => _membershipStore.Update(e, x => x.MembershipFunction = e.Function);

        public void Handle(MembershipResumed e)
            => _membershipStore.Insert(e);

        public void Handle(MembershipStarted e)
            => _membershipStore.Insert(e);

        public void Handle(MembershipStopped e)
            => _membershipStore.Delete(e.AggregateIdentifier);

        public void Handle(MembershipReasonAdded e)
            => _reasonStore.Insert(e);

        public void Handle(MembershipReasonModified e)
            => _reasonStore.Update(e);

        public void Handle(MembershipReasonRemoved e)
            => _reasonStore.Delete(e);
    }
}