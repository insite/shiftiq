using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class QGradebookEnrollmentAdapter : IEntityAdapter
{
    public void Copy(ModifyGradebookEnrollment modify, QGradebookEnrollmentEntity entity)
    {
        entity.GradebookIdentifier = modify.GradebookIdentifier;
        entity.LearnerIdentifier = modify.LearnerIdentifier;
        entity.PeriodIdentifier = modify.PeriodIdentifier;
        entity.EnrollmentStarted = modify.EnrollmentStarted;
        entity.EnrollmentComment = modify.EnrollmentComment;
        entity.EnrollmentRestart = modify.EnrollmentRestart;
        entity.EnrollmentCompleted = modify.EnrollmentCompleted;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public QGradebookEnrollmentEntity ToEntity(CreateGradebookEnrollment create)
    {
        var entity = new QGradebookEnrollmentEntity
        {
            GradebookIdentifier = create.GradebookIdentifier,
            LearnerIdentifier = create.LearnerIdentifier,
            PeriodIdentifier = create.PeriodIdentifier,
            EnrollmentIdentifier = create.EnrollmentIdentifier,
            EnrollmentStarted = create.EnrollmentStarted,
            EnrollmentComment = create.EnrollmentComment,
            EnrollmentRestart = create.EnrollmentRestart,
            EnrollmentCompleted = create.EnrollmentCompleted,
            OrganizationIdentifier = create.OrganizationIdentifier
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
            GradebookIdentifier = entity.GradebookIdentifier,
            LearnerIdentifier = entity.LearnerIdentifier,
            PeriodIdentifier = entity.PeriodIdentifier,
            EnrollmentIdentifier = entity.EnrollmentIdentifier,
            EnrollmentStarted = entity.EnrollmentStarted,
            EnrollmentComment = entity.EnrollmentComment,
            EnrollmentRestart = entity.EnrollmentRestart,
            EnrollmentCompleted = entity.EnrollmentCompleted,
            OrganizationIdentifier = entity.OrganizationIdentifier
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
            EnrollmentIdentifier = entity.EnrollmentIdentifier

        };

        return match;
    }
}