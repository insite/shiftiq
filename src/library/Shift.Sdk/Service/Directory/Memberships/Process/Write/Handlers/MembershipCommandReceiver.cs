using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Memberships.Write;
using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Write
{
    public class MembershipCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public MembershipCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<StartMembership>(Handle);
            commander.Subscribe<EndMembership>(Handle);
            commander.Subscribe<ResumeMembership>(Handle);
            commander.Subscribe<ModifyMembershipEffective>(Handle);
            commander.Subscribe<ModifyMembershipFunction>(Handle);

            commander.Subscribe<ModifyMembershipExpiry>(Handle);
            commander.Subscribe<ExpireMembership>(Handle);

            commander.Subscribe<AddMembershipReason>(Handle);
            commander.Subscribe<ModifyMembershipReason>(Handle);
            commander.Subscribe<RemoveMembershipReason>(Handle);
        }

        private void Commit(MembershipAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(StartMembership c)
        {
            var aggregate = new MembershipAggregate { AggregateIdentifier = c.AggregateIdentifier };

            aggregate.StartMembership(c.User, c.Group, c.Function, c.Effective);

            Commit(aggregate, c);
        }

        public void Handle(EndMembership c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.EndMembership();

                Commit(aggregate, c);
            });
        }

        public void Handle(ResumeMembership c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ResumeMembership(c.User, c.Group, c.Function, c.Effective);

                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyMembershipEffective c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Effective != c.Effective)
                {
                    aggregate.ModifyMembershipEffective(c.Effective);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ModifyMembershipFunction c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Function != c.Function)
                {
                    aggregate.ModifyMembershipFunction(c.Function);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ModifyMembershipExpiry c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (data.Expiry == c.Expiry)
                    return;

                aggregate.ModifyMembershipExpiry(c.Expiry);
                Commit(aggregate, c);
            });
        }

        public void Handle(ExpireMembership c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ExpireMembership(c.Expiry);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddMembershipReason c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.AddMembershipReason(c.ReasonIdentifier, c.Type, c.Subtype, c.Effective, c.Expiry, c.PersonOccupation);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyMembershipReason c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.ModifyMembershipReason(c.ReasonIdentifier, c.Type, c.Subtype, c.Effective, c.Expiry, c.PersonOccupation);
                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveMembershipReason c)
        {
            _repository.LockAndRun<MembershipAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.RemoveMembershipReason(c.ReasonIdentifier);
                Commit(aggregate, c);
            });
        }
    }
}
