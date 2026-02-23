using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class QGradebookEnrollmentAdapter : IEntityAdapter
{
    public void Copy(ModifyGradebookEnrollment modify, QGradebookEnrollmentEntity entity)
    {
        entity.GradebookIdentifier = modify.GradebookId;
        entity.LearnerIdentifier = modify.LearnerId;
        entity.PeriodIdentifier = modify.PeriodId;
        entity.EnrollmentStarted = modify.EnrollmentStarted;
        entity.EnrollmentComment = modify.EnrollmentComment;
        entity.EnrollmentRestart = modify.EnrollmentRestart;
        entity.EnrollmentCompleted = modify.EnrollmentCompleted;
        entity.OrganizationIdentifier = modify.OrganizationId;

    }

    public QGradebookEnrollmentEntity ToEntity(CreateGradebookEnrollment create)
    {
        var entity = new QGradebookEnrollmentEntity
        {
            GradebookIdentifier = create.GradebookId,
            LearnerIdentifier = create.LearnerId,
            PeriodIdentifier = create.PeriodId,
            EnrollmentIdentifier = create.EnrollmentId,
            EnrollmentStarted = create.EnrollmentStarted,
            EnrollmentComment = create.EnrollmentComment,
            EnrollmentRestart = create.EnrollmentRestart,
            EnrollmentCompleted = create.EnrollmentCompleted,
            OrganizationIdentifier = create.OrganizationId
        };
        return entity;
    }

    public IEnumerable<GradebookEnrollmentModel> ToModel(IEnumerable<QGradebookEnrollmentEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public GradebookEnrollmentModel ToModel(QGradebookEnrollmentEntity entity)
    {
        var model = new GradebookEnrollmentModel
        {
            GradebookId = entity.GradebookIdentifier,
            LearnerId = entity.LearnerIdentifier,
            PeriodId = entity.PeriodIdentifier,
            EnrollmentId = entity.EnrollmentIdentifier,
            EnrollmentStarted = entity.EnrollmentStarted,
            EnrollmentComment = entity.EnrollmentComment,
            EnrollmentRestart = entity.EnrollmentRestart,
            EnrollmentCompleted = entity.EnrollmentCompleted,
            OrganizationId = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<GradebookEnrollmentMatch> ToMatch(IEnumerable<QGradebookEnrollmentEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public GradebookEnrollmentMatch ToMatch(QGradebookEnrollmentEntity entity)
    {
        var match = new GradebookEnrollmentMatch
        {
            EnrollmentId = entity.EnrollmentIdentifier

        };

        return match;
    }
}