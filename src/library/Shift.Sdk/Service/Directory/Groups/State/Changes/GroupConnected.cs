using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class GroupConnected : Change
    {
        public Guid Group { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ConnectionType ConnectionType { get; }

        public GroupConnected(Guid group, ConnectionType connectionType)
        {
            Group = group;
            ConnectionType = connectionType;
        }
    }
}
