using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectGradebooks : Query<IEnumerable<GradebookModel>>, IGradebookCriteria
    {
        public Guid? AchievementId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? ClassInstructorId { get; set; }
        public Guid? FrameworkId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PeriodId { get; set; }

        public bool? IsLocked { get; set; }

        public string ClassTitle { get; set; }
        public string GradebookTitle { get; set; }
        public string GradebookType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string Reference { get; set; }

        public DateTimeOffset? ClassStartedSince { get; set; }
        public DateTimeOffset? ClassStartedBefore { get; set; }

        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }

        public DateTimeOffset? LastChangeSince { get; set; }
        public DateTimeOffset? LastChangeBefore { get; set; }
    }
}