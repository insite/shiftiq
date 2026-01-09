using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class AddPersonSecret : Command
    {
        public Guid PersonId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public int? Lifetime { get; set; }

        public AddPersonSecret(Guid personId, Guid secretId, string type, string name, string value, DateTimeOffset expiry, int? lifetime)
        {
            AggregateIdentifier = secretId;
            PersonId = personId;
            Type = type;
            Name = name;
            Value = value;
            Expiry = expiry;
            Lifetime = lifetime;
        }
    }
}
