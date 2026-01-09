using System;

namespace InSite.Domain.Standards
{
    public class StandardGroup
    {
        public Guid GroupId { get; set; }
        public DateTimeOffset? Assigned { get; set; }

        public StandardGroup()
        {

        }

        public StandardGroup(Guid groupId, DateTimeOffset? assigned = null)
        {
            GroupId = groupId;
            Assigned = assigned;
        }

        public StandardGroup Clone()
        {
            return (StandardGroup)MemberwiseClone();
        }
    }
}
