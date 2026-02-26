using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCredentials : Query<IEnumerable<CredentialModel>>, ICredentialCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? LearnerId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}