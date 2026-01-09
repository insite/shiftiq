using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class GradebookAdapter : IEntityAdapter
{
    public void Copy(ModifyGradebook modify, GradebookEntity entity)
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

    public IEnumerable<GradebookModel> ToModel(IEnumerable<GradebookEntity> entities, TimeZoneInfo timezone)
    {
        return entities.Select(e => ToModel(e, timezone));
    }

    public GradebookModel ToModel(GradebookEntity entity, TimeZoneInfo timezone)
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