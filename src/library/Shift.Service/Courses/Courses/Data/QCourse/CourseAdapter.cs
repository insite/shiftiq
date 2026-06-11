using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Learning;

public class CourseAdapter : IEntityAdapter
{
    public void Copy(ModifyCourse modify, CourseEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.FrameworkStandardIdentifier = modify.FrameworkStandardId;
        entity.GradebookIdentifier = modify.GradebookId;
        entity.CatalogIdentifier = modify.CatalogId;
        entity.CompletionActivityIdentifier = modify.CompletionActivityId;
        entity.CompletedToLearnerMessageIdentifier = modify.CompletedToLearnerMessageId;
        entity.CompletedToAdministratorMessageIdentifier = modify.CompletedToAdministratorMessageId;
        entity.StalledToLearnerMessageIdentifier = modify.StalledToLearnerMessageId;
        entity.StalledToAdministratorMessageIdentifier = modify.StalledToAdministratorMessageId;
        entity.CourseAsset = modify.CourseAsset;
        entity.CourseName = modify.CourseName;
        entity.CourseHook = modify.CourseHook;
        entity.CourseCode = modify.CourseCode;
        entity.CourseImage = modify.CourseImage;
        entity.CoursePlatform = modify.CoursePlatform;
        entity.SourceIdentifier = modify.SourceId;
        entity.CreatedBy = modify.CreatedBy;
        entity.Created = modify.Created;
        entity.ModifiedBy = modify.ModifiedBy;
        entity.Modified = modify.Modified;
        entity.CourseIsHidden = modify.CourseIsHidden;
        entity.CourseLabel = modify.CourseLabel;
        entity.CourseProgram = modify.CourseProgram;
        entity.CourseLevel = modify.CourseLevel;
        entity.CourseUnit = modify.CourseUnit;
        entity.CourseDescription = modify.CourseDescription;
        entity.CourseStyle = modify.CourseStyle;
        entity.IsMultipleUnitsEnabled = modify.IsMultipleUnitsEnabled;
        entity.CourseSequence = modify.CourseSequence;
        entity.CourseIcon = modify.CourseIcon;
        entity.CourseSlug = modify.CourseSlug;
        entity.SendMessageStalledAfterDays = modify.SendMessageStalledAfterDays;
        entity.SendMessageStalledMaxCount = modify.SendMessageStalledMaxCount;
        entity.IsProgressReportEnabled = modify.IsProgressReportEnabled;
        entity.OutlineWidth = modify.OutlineWidth;
        entity.AllowDiscussion = modify.AllowDiscussion;
        entity.CourseFlagColor = modify.CourseFlagColor;
        entity.CourseFlagText = modify.CourseFlagText;
        entity.Closed = modify.Closed;
        entity.IsDisplayOverviewOnly = modify.IsDisplayOverviewOnly;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CourseEntity ToEntity(CreateCourse create)
    {
        var entity = new CourseEntity
        {
            CourseIdentifier = create.CourseId,
            OrganizationIdentifier = create.OrganizationId,
            FrameworkStandardIdentifier = create.FrameworkStandardId,
            GradebookIdentifier = create.GradebookId,
            CatalogIdentifier = create.CatalogId,
            CompletionActivityIdentifier = create.CompletionActivityId,
            CompletedToLearnerMessageIdentifier = create.CompletedToLearnerMessageId,
            CompletedToAdministratorMessageIdentifier = create.CompletedToAdministratorMessageId,
            StalledToLearnerMessageIdentifier = create.StalledToLearnerMessageId,
            StalledToAdministratorMessageIdentifier = create.StalledToAdministratorMessageId,
            CourseAsset = create.CourseAsset,
            CourseName = create.CourseName,
            CourseHook = create.CourseHook,
            CourseCode = create.CourseCode,
            CourseImage = create.CourseImage,
            CoursePlatform = create.CoursePlatform,
            SourceIdentifier = create.SourceId,
            CreatedBy = create.CreatedBy,
            Created = create.Created,
            ModifiedBy = create.ModifiedBy,
            Modified = create.Modified,
            CourseIsHidden = create.CourseIsHidden,
            CourseLabel = create.CourseLabel,
            CourseProgram = create.CourseProgram,
            CourseLevel = create.CourseLevel,
            CourseUnit = create.CourseUnit,
            CourseDescription = create.CourseDescription,
            CourseStyle = create.CourseStyle,
            IsMultipleUnitsEnabled = create.IsMultipleUnitsEnabled,
            CourseSequence = create.CourseSequence,
            CourseIcon = create.CourseIcon,
            CourseSlug = create.CourseSlug,
            SendMessageStalledAfterDays = create.SendMessageStalledAfterDays,
            SendMessageStalledMaxCount = create.SendMessageStalledMaxCount,
            IsProgressReportEnabled = create.IsProgressReportEnabled,
            OutlineWidth = create.OutlineWidth,
            AllowDiscussion = create.AllowDiscussion,
            CourseFlagColor = create.CourseFlagColor,
            CourseFlagText = create.CourseFlagText,
            Closed = create.Closed,
            IsDisplayOverviewOnly = create.IsDisplayOverviewOnly
        };
        return entity;
    }

    public IEnumerable<CourseModel> ToModel(IEnumerable<CourseEntity> entities, TimeZoneInfo? timezone)
    {
        return entities.Select(e => ToModel(e, timezone));
    }

    public CourseModel ToModel(CourseEntity entity, TimeZoneInfo? timezone)
    {
        var model = new CourseModel
        {
            CourseId = entity.CourseIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            FrameworkStandardId = entity.FrameworkStandardIdentifier,
            GradebookId = entity.GradebookIdentifier,
            CatalogId = entity.CatalogIdentifier,
            CompletionActivityId = entity.CompletionActivityIdentifier,
            CompletedToLearnerMessageId = entity.CompletedToLearnerMessageIdentifier,
            CompletedToAdministratorMessageId = entity.CompletedToAdministratorMessageIdentifier,
            StalledToLearnerMessageId = entity.StalledToLearnerMessageIdentifier,
            StalledToAdministratorMessageId = entity.StalledToAdministratorMessageIdentifier,
            CourseAsset = entity.CourseAsset,
            CourseName = entity.CourseName,
            CourseHook = entity.CourseHook,
            CourseCode = entity.CourseCode,
            CourseImage = entity.CourseImage,
            CoursePlatform = entity.CoursePlatform,
            SourceId = entity.SourceIdentifier,
            CreatedBy = entity.CreatedBy,
            Created = entity.Created,
            ModifiedBy = entity.ModifiedBy,
            Modified = entity.Modified,
            CourseIsHidden = entity.CourseIsHidden,
            CourseLabel = entity.CourseLabel,
            CourseProgram = entity.CourseProgram,
            CourseLevel = entity.CourseLevel,
            CourseUnit = entity.CourseUnit,
            CourseDescription = entity.CourseDescription,
            CourseStyle = entity.CourseStyle,
            IsMultipleUnitsEnabled = entity.IsMultipleUnitsEnabled,
            CourseSequence = entity.CourseSequence,
            CourseIcon = entity.CourseIcon,
            CourseSlug = entity.CourseSlug,
            SendMessageStalledAfterDays = entity.SendMessageStalledAfterDays,
            SendMessageStalledMaxCount = entity.SendMessageStalledMaxCount,
            IsProgressReportEnabled = entity.IsProgressReportEnabled,
            OutlineWidth = entity.OutlineWidth,
            AllowDiscussion = entity.AllowDiscussion,
            CourseFlagColor = entity.CourseFlagColor,
            CourseFlagText = entity.CourseFlagText,
            Closed = entity.Closed,
            IsDisplayOverviewOnly = entity.IsDisplayOverviewOnly
        };

        return model;
    }
}