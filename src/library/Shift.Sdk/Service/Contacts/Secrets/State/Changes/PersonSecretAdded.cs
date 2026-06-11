using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonSecretAdded : Change
    {
        public Guid PersonId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public int? Lifetime { get; set; }

        public PersonSecretAdded(Guid personId, string type, string name, string value, DateTimeOffset expiry, int? lifetime)
        {
            PersonId = personId;
            Type = type;
            Name = name;
            Value = value;
            Expiry = expiry;
            Lifetime = lifetime;
        }
    }
}