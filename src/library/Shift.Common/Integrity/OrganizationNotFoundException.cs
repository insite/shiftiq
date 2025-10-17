using System;

namespace Shift.Common
{
    [Serializable]
    public class OrganizationNotFoundException : Exception
    {
        public OrganizationNotFoundException(Guid organization) : base($"Organization Not Found: {organization}") { }

        public OrganizationNotFoundException(string organization) : base($"Organization Not Found: {organization}") { }

        public OrganizationNotFoundException(string organization, Exception inner) : base($"Organization Not Found: {organization}", inner) { }
    }
}
