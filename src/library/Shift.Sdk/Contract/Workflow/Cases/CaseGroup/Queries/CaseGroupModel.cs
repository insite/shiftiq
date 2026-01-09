using System;

namespace Shift.Contract
{
    public partial class CaseGroupModel
    {
        public Guid GroupIdentifier { get; set; }
        public Guid CaseIdentifier { get; set; }
        public Guid JoinIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}