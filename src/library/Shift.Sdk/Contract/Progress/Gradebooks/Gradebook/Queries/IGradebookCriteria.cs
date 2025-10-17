using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGradebookCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementIdentifier { get; set; }

        Guid? ClassIdentifier { get; set; }

        Guid? ClassInstructorIdentifier { get; set; }

        Guid? FrameworkIdentifier { get; set; }

        Guid OrganizationIdentifier { get; set; }

        Guid? PeriodIdentifier { get; set; }

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