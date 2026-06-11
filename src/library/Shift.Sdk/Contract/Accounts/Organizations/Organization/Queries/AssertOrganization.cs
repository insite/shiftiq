using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertOrganization : Query<bool>
    {
        public Guid OrganizationId { get; set; }
    }
}