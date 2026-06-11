using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class PersonSecretAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new PersonSecretState();

        public PersonSecretState Data => (PersonSecretState)State;

        public void AddPersonSecret(Guid personId, string type, string name, string value, DateTimeOffset expiry, int? lifetime)
        {
            Apply(new PersonSecretAdded(personId, type, name, value, expiry, lifetime));
        }

        public void RemovePersonSecret(Guid userId, Guid organizationId)
        {
            Apply(new PersonSecretRemoved(userId, organizationId));
        }
    }
}
