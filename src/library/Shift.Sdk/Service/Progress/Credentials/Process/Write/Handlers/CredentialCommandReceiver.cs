using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class CredentialCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public CredentialCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<CreateCredential>(Handle);
            commander.Subscribe<CreateAndGrantCredential>(Handle);

            commander.Subscribe<ChangeCredentialAuthority>(Handle);
            commander.Subscribe<ChangeCredentialExpiration>(Handle);
            commander.Subscribe<ChangeCredentialEmployer>(Handle);
            commander.Subscribe<TagCredential>(Handle);

            commander.Subscribe<DescribeCredential>(Handle);
            commander.Subscribe<GrantCredential>(Handle);
            commander.Subscribe<ExpireCredential>(Handle);
            commander.Subscribe<RevokeCredential>(Handle);
            commander.Subscribe<DeleteCredential>(Handle);

            commander.Subscribe<RequestExpirationReminder>(Handle);
            commander.Subscribe<DeliverExpirationReminder>(Handle);
        }

        private void Commit(CredentialAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(CreateCredential c)
        {
            var aggregate = !_repository.Exists<CredentialAggregate>(c.AggregateIdentifier)
                ? new CredentialAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Achievement }
                : _repository.Get<CredentialAggregate>(c.AggregateIdentifier);

            aggregate.AssignCredential(c.Tenant, c.Achievement, c.User, c.Assigned);

            Commit(aggregate, c);
        }

        public void Handle(CreateAndGrantCredential c)
        {
            Expiration expiration;
            CredentialAggregate credential;

            if (!_repository.Exists<CredentialAggregate>(c.AggregateIdentifier))
            {
                credential = new CredentialAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Achievement };
                var achievement = _repository.Get<AchievementAggregate>(c.Achievement);
                expiration = achievement.Data.Expiration;
            }
            else
            {
                credential = _repository.Get<CredentialAggregate>(c.AggregateIdentifier);
                expiration = credential.Data.Expiration;
            }

            credential.AssignAndGrantCredential(
                c.Tenant,
                c.Achievement,
                c.User,
                c.Granted,
                c.Description,
                c.Score,
                expiration,
                c.EmployerGroup,
                c.EmployerGroupStatus
                );

            Commit(credential, c);
        }

        public void Handle(ChangeCredentialAuthority c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeCredentialAuthority(c.AuthorityIdentifier, c.AuthorityName, c.AuthorityType, c.Location, c.Reference, c.Hours);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeCredentialExpiration c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeCredentialExpiration(c.Expiration);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeCredentialEmployer c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.EmployerGroup != c.EmployerGroup
                    || aggregate.Data.EmployerGroupStatus != c.EmployerGroupStatus
                    )
                {
                    aggregate.ChangeCredentialEmployer(c.EmployerGroup, c.EmployerGroupStatus);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(TagCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TagCredential(c.Necessity, c.Priority);
                Commit(aggregate, c);
            });
        }

        public void Handle(DescribeCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DescribeCredential(c.Description);
                Commit(aggregate, c);
            });
        }

        public void Handle(GrantCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.GrantCredential(c.Granted, c.Description, c.Score, c.EmployerGroup, c.EmployerGroupStatus);
                Commit(aggregate, c);
            });
        }

        public void Handle(ExpireCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ExpireCredential(c.Expired);
                Commit(aggregate, c);
            });
        }

        public void Handle(RevokeCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RevokeCredential(c.Revoked, c.Reason, c.Score);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteCredential c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteCredential();
                Commit(aggregate, c);
            });
        }

        public void Handle(RequestExpirationReminder c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RequestExpirationReminder(c.Type, c.Requested);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeliverExpirationReminder c)
        {
            _repository.LockAndRun<CredentialAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeliverExpirationReminder(c.Type, c.Delivered);
                Commit(aggregate, c);
            });
        }
    }
}
