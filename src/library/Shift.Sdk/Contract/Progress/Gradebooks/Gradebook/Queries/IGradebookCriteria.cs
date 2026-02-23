using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGradebookCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementId { get; set; }

        Guid? ClassId { get; set; }

        Guid? ClassInstructorId { get; set; }

        Guid? FrameworkId { get; set; }

        Guid? PeriodId { get; set; }

        bool? IsLocked { get; set; }

        string ClassTitle { get; set; }

        string GradebookTitle { get; set; }

        string GradebookType { get; set; }

        string LastChangeType { get; set; }

        string LastChangeUser { get; set; }

        string Reference { get; set; }

        DateTimeOffset? ClassStartedSince { get; set; }
        DateTimeOffset? ClassStartedBefore { get; set; }

        DateTimeOffset? GradebookCreatedSince { get; set; }
        DateTimeOffset? GradebookCreatedBefore { get; set; }

        DateTimeOffset? LastChangeSince { get; set; }
        DateTimeOffset? LastChangeBefore { get; set; }
    }
}