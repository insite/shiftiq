using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGradebooks : Query<IEnumerable<GradebookMatch>>, IGradebookCriteria
    {
        public Guid? OrganizationId { get; set; }

        // Column 1 criteria
        public string GradebookTitle { get; set; }
        public DateTimeOffset? GradebookCreatedSince { get; set; }
        public DateTimeOffset? GradebookCreatedBefore { get; set; }
        public Guid? PeriodId { get; set; }

        // Column 2 criteria
        public Guid? AchievementId { get; set; }
        public Guid? FrameworkId { get; set; }
        public bool? IsLocked { get; set; }

        // Column 3 criteria
        public string ClassTitle { get; set; }
        public DateTimeOffset? ClassStartedSince { get; set; }
        public DateTimeOffset? ClassStartedBefore { get; set; }
        public Guid? ClassInstructorId { get; set; }

        // Additional criteria (no UI)
        public Guid? ClassId { get; set; }
        public string GradebookType { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string Reference { get; set; }
        public DateTimeOffset? LastChangeSince { get; set; }
        public DateTimeOffset? LastChangeBefore { get; set; }
    }
}