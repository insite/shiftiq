using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationCreated : Change
    {
        public DateTimeOffset? Opened { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public OrganizationCreated(DateTimeOffset? opened, string code, string name)
        {
            Opened = opened;
            Code = code;
            Name = name;
        }
    }
}
