using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalSetupUserAdded : Change
    {
        public Guid User { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public JournalSetupUserRole Role { get; }

        public JournalSetupUserAdded(Guid user, JournalSetupUserRole role)
        {
            User = user;
            Role = role;
        }
    }
}
