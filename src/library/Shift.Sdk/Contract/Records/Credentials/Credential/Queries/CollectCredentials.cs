using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCredentials : Query<IEnumerable<CredentialModel>>, ICredentialCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
