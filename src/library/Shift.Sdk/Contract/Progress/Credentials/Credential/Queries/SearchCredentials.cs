using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCredentials : Query<IEnumerable<CredentialMatch>>, ICredentialCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? LearnerId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}