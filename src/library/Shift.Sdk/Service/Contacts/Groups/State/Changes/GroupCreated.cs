using System;

using Shift.Common.Timeline.Changes;


namespace InSite.Domain.Contacts
{
    public class GroupCreated : Change
    {
        public Guid Tenant { get; }
        public string Type { get; }
        public string Name { get; }
        public DateTimeOffset Created { get; }

        public GroupCreated(Guid tenant, string type, string name, DateTimeOffset created)
        {
            Tenant = tenant;
            Type = type;
            Name = name;
            Created = created;
        }
    }
}
