namespace Shift.Service.Gradebook;

using Shift.Common;
using Shift.Contract;

public class QGradebookAdapter : IEntityAdapter
{
    public void Copy(ModifyGradebook modify, QGradebookEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.EventIdentifier = modify.EventIdentifier;
        entity.AchievementIdentifier = modify.AchievementIdentifier;
        entity.FrameworkIdentifier = modify.FrameworkIdentifier;
        entity.GradebookCreated = modify.GradebookCreated;
        entity.GradebookTitle = modify.GradebookTitle;
        entity.GradebookType = modify.GradebookType;
        entity.IsLocked = modify.IsLocked;
        entity.Reference = modify.Reference;
        entity.PeriodIdentifier = modify.PeriodIdentifier;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
    }

    public string Serialize(IEnumerable<GradebookModel> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public QGradebookEntity ToEntity(CreateGradebook create)
    {
        var entity = new QGradebookEntity
        {
            GradebookIdentifier = create.GradebookIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            EventIdentifier = create.EventIdentifier,
            AchievementIdentifier = create.AchievementIdentifier,
            FrameworkIdentifier = create.FrameworkIdentifier,
            GradebookCreated = create.GradebookCreated,
            GradebookTitle = create.GradebookTitle,
            GradebookType = create.GradebookType,
            IsLocked = create.IsLocked,
            Reference = create.Reference,
            PeriodIdentifier = create.PeriodIdentifier,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser
        };
        return entity;
    }

    public IEnumerable<GradebookModel> ToModel(IEnumerable<QGradebookEntity> entities, TimeZoneInfo timeZone)
    {
        return entities.Select(e => ToModel(e, timeZone));
    }

    public GradebookModel ToModel(QGradebookEntity entity, TimeZoneInfo timeZone)
    {
        var model = new GradebookModel
        {
            GradebookIdentifier = entity.GradebookIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            EventIdentifier = entity.EventIdentifier,
            AchievementIdentifier = entity.AchievementIdentifier,
            FrameworkIdentifier = entity.FrameworkIdentifier,
            GradebookCreated = entity.GradebookCreated,
            GradebookTitle = entity.GradebookTitle,
            GradebookType = entity.GradebookType,
            IsLocked = entity.IsLocked,
            Reference = entity.Reference,
            PeriodIdentifier = entity.PeriodIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser
        };

        if (entity.Achievement != null)
        {
            model.AchievementTitle = entity.Achievement.AchievementTitle;
        }

        if (entity.Event != null)
        {
            model.ClassInstructors = null;

            if (entity.Event.EventScheduledEnd != null)
                model.ClassScheduledEndDate = Common.TimeZones.FormatDateOnly(entity.Event.EventScheduledEnd.Value, timeZone);

            model.ClassScheduledStartDate = Common.TimeZones.FormatDateOnly(entity.Event.EventScheduledStart, timeZone);

            model.ClassTitle = entity.Event.EventTitle;
        }

        return model;
    }

    public IEnumerable<GradebookMatch> ToMatch(IEnumerable<QGradebookEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public GradebookMatch ToMatch(QGradebookEntity entity)
    {
        var match = new GradebookMatch
        {
            GradebookIdentifier = entity.GradebookIdentifier,
            GradebookTitle = entity.GradebookTitle,
            GradebookCreated = entity.GradebookCreated,
            GradebookEnrollmentCount = entity.Enrollments.Count,

            ClassIdentifier = entity.EventIdentifier,
            ClassTitle = entity.Event?.EventTitle,
            ClassStarted = entity.Event?.EventScheduledStart,
            ClassEnded = entity.Event?.EventScheduledEnd,

            AchievementIdentifier = entity.AchievementIdentifier,
            AchievementTitle = entity.Achievement?.AchievementTitle,
            AchievementCountGranted = entity.Achievement?.Credentials?.Count ?? 0
        };

        return match;
    }
}