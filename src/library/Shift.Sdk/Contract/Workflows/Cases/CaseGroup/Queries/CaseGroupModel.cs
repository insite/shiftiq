using System;

namespace Shift.Contract
{
    public partial class CaseGroupModel
    {
        public Guid GroupId { get; set; }
        public Guid CaseId { get; set; }
        public Guid JoinId { get; set; }
        public Guid OrganizationId { get; set; }

        public string CaseRole { get; set; }
    }
}