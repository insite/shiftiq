using System;

namespace InSite.Persistence
{
    public class TScormStatement
    {
        public Guid RegistrationIdentifier { get; set; }
        public Guid StatementIdentifier { get; set; }

        public string ActorName { get; set; }
        public string ObjectDefinitionName { get; set; }
        public string StatementData { get; set; }
        public string VerbDisplay { get; set; }

        public DateTimeOffset? StatementTimestamp { get; set; }

        public virtual TScormRegistration Registration { get; set; }
    }
}
