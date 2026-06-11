using System;

namespace Shift.Contract
{
    public partial class CaseUserModel
    {
        public Guid CaseId { get; set; }
        public Guid JoinId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }

        public string CaseRole { get; set; }
    }
}