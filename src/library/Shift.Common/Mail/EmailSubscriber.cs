using System;
using System.Collections.Generic;

namespace Shift.Common
{
    [Serializable]
    public class EmailSubscriber
    {
        public DateTimeOffset? Subscribed { get; set; }

        public Guid UserIdentifier { get; set; }
        public string UserEmail { get; set; }
        public bool UserEmailEnabled { get; set; }
        public string UserEmailAlternate { get; set; }
        public bool UserEmailAlternateEnabled { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserFullName { get; set; }
        public string PersonCode { get; set; }
        public string Language { get; set; }

        public List<Guid> Followers { get; set; } = new List<Guid>();
    }
}