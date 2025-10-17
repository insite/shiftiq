using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveOrganization : Query<OrganizationModel>
    {
        public Guid OrganizationIdentifier { get; set; }
    }
}