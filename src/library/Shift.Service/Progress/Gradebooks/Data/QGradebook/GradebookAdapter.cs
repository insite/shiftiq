using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class GradebookAdapter : IEntityAdapter
{
    public void Copy(ModifyGradebook modify, GradebookEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.EventIdentifier = modify.EventId;
        entity.AchievementIdentifier = modify.AchievementId;
        entity.FrameworkIdentifier = modify.FrameworkId;
        entity.GradebookCreated = modify.GradebookCreated;
        entity.GradebookTitle = modify.GradebookTitle;
        entity.GradebookType = modify.GradebookType;
        entity.IsLocked = modify.IsLocked;
        entity.Reference = modify.Reference;
        entity.PeriodIdentifier = modify.PeriodId;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public GradebookEntity ToEntity(CreateGradebook create)
    {
        var entity = new GradebookEntity
        {
            GradebookIdentifier = create.GradebookId,
            OrganizationIdentifier = create.OrganizationId,
            EventIdentifier = create.EventId,
            AchievementIdentifier = create.AchievementId,
            FrameworkIdentifier = create.FrameworkId,
            GradebookCreated = create.GradebookCreated,
            GradebookTitle = create.GradebookTitle,
            GradebookType = create.GradebookType,
            IsLocked = create.IsLocked,
            Reference = create.Reference,
            PeriodIdentifier = create.PeriodId,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser
        };
        return entity;
    }

    public IEnumerable<GradebookModel> ToModel(IEnumerable<GradebookEntity> entities, TimeZoneInfo? timezone)
    {
        return entities.Select(e => ToModel(e, timezone));
    }

    public GradebookModel ToModel(GradebookEntity entity, TimeZoneInfo? timezone)
    {
        var model = new GradebookModel
        {
            GradebookId = entity.GradebookIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            EventId = entity.EventIdentifier,
            AchievementId = entity.AchievementIdentifier,
            FrameworkId = entity.FrameworkIdentifier,
            GradebookCreated = entity.GradebookCreated,
            GradebookTitle = entity.GradebookTitle,
            GradebookType = entity.GradebookType,
            IsLocked = entity.IsLocked,
            Reference = entity.Reference,
            PeriodId = entity.PeriodIdentifier,
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
                model.ClassScheduledEndDate = Common.TimeZones.FormatDateOnly(entity.Event.EventScheduledEnd.Value, timezone);

            model.ClassScheduledStartDate = Common.TimeZones.FormatDateOnly(entity.Event.EventScheduledStart, timezone);

            model.ClassTitle = entity.Event.EventTitle;
        }

        return model;
    }

    public IEnumerable<GradebookMatch> ToMatch(IEnumerable<GradebookEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public GradebookMatch ToMatch(GradebookEntity entity)
    {
        var match = new GradebookMatch
        {
            GradebookId = entity.GradebookIdentifier,
            GradebookTitle = entity.GradebookTitle,
            GradebookCreated = entity.GradebookCreated,
            GradebookEnrollmentCount = entity.Enrollments.Count,

            ClassId = entity.EventIdentifier,
            ClassTitle = entity.Event?.EventTitle,
            ClassStarted = entity.Event?.EventScheduledStart,
            ClassEnded = entity.Event?.EventScheduledEnd,

            AchievementId = entity.AchievementIdentifier,
            AchievementTitle = entity.Achievement?.AchievementTitle,
            AchievementCountGranted = entity.Achievement?.Credentials?.Count ?? 0
        };

        return match;
    }
}