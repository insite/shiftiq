using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    [Serializable]
    public class PersonSecretState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid Person { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public int? Lifetime { get; set; }

        public void When(PersonSecretAdded e)
        {
            Identifier = e.AggregateIdentifier;
            Person = e.PersonId;
            Lifetime = e.Lifetime;
            Type = e.Type;
            Name = e.Name;
            Value = e.Value;
            Expiry = e.Expiry;
            Lifetime = e.Lifetime;
        }

        public void When(PersonSecretRemoved _)
        {
        }
    }
}
