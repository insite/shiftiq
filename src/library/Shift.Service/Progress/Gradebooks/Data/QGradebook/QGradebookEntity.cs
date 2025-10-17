using Shift.Service.Achievement;
using Shift.Service.Booking;

namespace Shift.Service.Gradebook;

public partial class QGradebookEntity
{
    public ICollection<QGradebookEnrollmentEntity> Enrollments { get; set; } = new List<QGradebookEnrollmentEntity>();

    public QAchievementEntity? Achievement { get; set; }
    public QEventEntity? Event { get; set; }

    public Guid? AchievementIdentifier { get; set; }
    public Guid? EventIdentifier { get; set; }
    public Guid? FrameworkIdentifier { get; set; }
    public Guid GradebookIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? PeriodIdentifier { get; set; }

    public bool IsLocked { get; set; }

    public string GradebookTitle { get; set; } = null!;
    public string GradebookType { get; set; } = null!;
    public string LastChangeType { get; set; } = null!;
    public string LastChangeUser { get; set; } = null!;
    public string? Reference { get; set; }

    public DateTimeOffset GradebookCreated { get; set; }
    public DateTimeOffset LastChangeTime { get; set; }
}