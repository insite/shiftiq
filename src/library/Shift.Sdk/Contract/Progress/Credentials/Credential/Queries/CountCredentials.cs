using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountCredentials : Query<int>, ICredentialCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? LearnerId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}