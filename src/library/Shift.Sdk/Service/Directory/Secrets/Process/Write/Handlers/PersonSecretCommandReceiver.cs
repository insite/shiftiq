using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Write
{
    public class PersonSecretCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public PersonSecretCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository
            )
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddPersonSecret>(Handle);
            commander.Subscribe<RemovePersonSecret>(Handle);
        }

        private void Commit(PersonSecretAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddPersonSecret c)
        {
            var aggregate = new PersonSecretAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.PersonId };

            aggregate.AddPersonSecret(c.PersonId, c.Type, c.Name, c.Value, c.Expiry, c.Lifetime);

            Commit(aggregate, c);
        }

        public void Handle(RemovePersonSecret c)
        {
            _repository.LockAndRun<PersonSecretAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RemovePersonSecret(c.PersonId, c.OrganizationId);
                Commit(aggregate, c);
            });
        }
    }
}
