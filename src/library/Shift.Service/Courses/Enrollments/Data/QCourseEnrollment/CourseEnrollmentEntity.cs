namespace Shift.Service.Learning;

public class CourseEnrollmentEntity
{
    public Guid CourseEnrollmentIdentifier { get; set; }
    public Guid CourseIdentifier { get; set; }
    public Guid LearnerUserIdentifier { get; set; }
    public Guid OrganizationIdentifier { get; set; }

    public int MessageCompletedSentCount { get; set; }
    public int MessageStalledSentCount { get; set; }

    public DateTimeOffset? CourseCompleted { get; set; }
    public DateTimeOffset CourseStarted { get; set; }
}