using System;

namespace Shift.Contract
{
    public class ModifyCaseUser
    {
        public Guid CaseIdentifier { get; set; }
        public Guid JoinIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string CaseRole { get; set; }
    }
}