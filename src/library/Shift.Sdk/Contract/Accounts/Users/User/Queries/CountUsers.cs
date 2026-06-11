using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUsers : Query<int>, IUserCriteria
    {
        public Guid? OrganizationId { get; set; }

        public Guid[] UserIds { get; set; }
        public string UserEmailExact { get; set; }
        public string UserFullNameContains { get; set; }
    }
}