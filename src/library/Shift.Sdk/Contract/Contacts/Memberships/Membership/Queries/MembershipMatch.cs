using System;

namespace Shift.Contract
{
    public partial class MembershipMatch
    {
        public Guid GroupId { get; set; }
        public Guid MembershipId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }

        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
    }
}
