using System;

using Newtonsoft.Json;

using Shift.Common.Timeline.Changes;
using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalSetupGroupRemoved : Change
    {
        public Guid Group { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public JournalSetupUserRole Role { get; }

        public JournalSetupGroupRemoved(Guid group, JournalSetupUserRole role)
        {
            Group = group;
            Role = role;
        }
    }
}
