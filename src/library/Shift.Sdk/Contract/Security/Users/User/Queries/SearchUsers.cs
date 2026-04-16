using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchUsers : Query<IEnumerable<UserMatch>>, IUserCriteria
    {
        public Guid? OrganizationId { get; set; }
        
        public Guid[] UserIds { get; set; }
        public string UserEmailExact { get; set; }
        public string UserFullNameContains { get; set; }
    }
}