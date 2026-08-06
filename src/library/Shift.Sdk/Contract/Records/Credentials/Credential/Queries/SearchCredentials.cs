using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCredentials : Query<IEnumerable<CredentialMatch>>, ICredentialCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTimeOffset? ModifiedFrom { get; set; }
        public DateTimeOffset? ModifiedBefore { get; set; }
        public string PersonCode { get; set; }
    }
}
