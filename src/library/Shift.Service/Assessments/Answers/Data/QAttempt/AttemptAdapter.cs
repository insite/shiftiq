using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptAdapter : IEntityAdapter
{
    public void Copy(ModifyAttempt modify, AttemptEntity entity)
    {
        entity.AttemptGraded = modify.AttemptGraded;
        entity.AttemptDuration = modify.AttemptDuration;
        entity.AttemptGrade = modify.AttemptGrade;
        entity.AttemptImported = modify.AttemptImported;
        entity.AttemptIsPassing = modify.AttemptIsPassing;
        entity.AttemptNumber = modify.AttemptNumber;
        entity.AttemptPinged = modify.AttemptPinged;
        entity.AttemptPoints = modify.AttemptPoints;
        entity.AttemptScore = modify.AttemptScore;
        entity.AttemptStarted = modify.AttemptStarted;
        entity.AttemptStatus = modify.AttemptStatus;
        entity.AttemptTag = modify.AttemptTag;
        entity.AttemptTimeLimit = modify.AttemptTimeLimit;
        entity.AssessorUserIdentifier = modify.AssessorUserId;
        entity.FormIdentifier = modify.FormId;
        entity.FormPoints = modify.FormPoints;
        entity.RegistrationIdentifier = modify.RegistrationId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.UserAgent = modify.UserAgent;
        entity.AttemptSubmitted = modify.AttemptSubmitted;
        entity.LearnerUserIdentifier = modify.LearnerUserId;
        entity.GradingAssessorUserIdentifier = modify.GradingAssessorUserId;
        entity.SectionsAsTabsEnabled = modify.SectionsAsTabsEnabled;
        entity.TabNavigationEnabled = modify.TabNavigationEnabled;
        entity.FormSectionsCount = modify.FormSectionsCount;
        entity.ActiveSectionIndex = modify.ActiveSectionIndex;
        entity.SingleQuestionPerTabEnabled = modify.SingleQuestionPerTabEnabled;
        entity.ActiveQuestionIndex = modify.ActiveQuestionIndex;
        entity.TabTimeLimit = modify.TabTimeLimit;
        entity.AttemptPingInterval = modify.AttemptPingInterval;
        entity.AttemptLanguage = modify.AttemptLanguage;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptEntity ToEntity(CreateAttempt create)
    {
        var entity = new AttemptEntity
        {
            AttemptGraded = create.AttemptGraded,
            AttemptDuration = create.AttemptDuration,
            AttemptGrade = create.AttemptGrade,
            AttemptIdentifier = create.AttemptId,
            AttemptImported = create.AttemptImported,
            AttemptIsPassing = create.AttemptIsPassing,
            AttemptNumber = create.AttemptNumber,
            AttemptPinged = create.AttemptPinged,
            AttemptPoints = create.AttemptPoints,
            AttemptScore = create.AttemptScore,
            AttemptStarted = create.AttemptStarted,
            AttemptStatus = create.AttemptStatus,
            AttemptTag = create.AttemptTag,
            AttemptTimeLimit = create.AttemptTimeLimit,
            AssessorUserIdentifier = create.AssessorUserId,
            FormIdentifier = create.FormId,
            FormPoints = create.FormPoints,
            RegistrationIdentifier = create.RegistrationId,
            OrganizationIdentifier = create.OrganizationId,
            UserAgent = create.UserAgent,
            AttemptSubmitted = create.AttemptSubmitted,
            LearnerUserIdentifier = create.LearnerUserId,
            GradingAssessorUserIdentifier = create.GradingAssessorUserId,
            SectionsAsTabsEnabled = create.SectionsAsTabsEnabled,
            TabNavigationEnabled = create.TabNavigationEnabled,
            FormSectionsCount = create.FormSectionsCount,
            ActiveSectionIndex = create.ActiveSectionIndex,
            SingleQuestionPerTabEnabled = create.SingleQuestionPerTabEnabled,
            ActiveQuestionIndex = create.ActiveQuestionIndex,
            TabTimeLimit = create.TabTimeLimit,
            AttemptPingInterval = create.AttemptPingInterval,
            AttemptLanguage = create.AttemptLanguage
        };
        return entity;
    }

    public IEnumerable<AttemptModel> ToModel(IEnumerable<AttemptEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptModel ToModel(AttemptEntity entity)
    {
        var model = new AttemptModel
        {
            AttemptGraded = entity.AttemptGraded,
            AttemptDuration = entity.AttemptDuration,
            AttemptGrade = entity.AttemptGrade,
            AttemptId = entity.AttemptIdentifier,
            AttemptImported = entity.AttemptImported,
            AttemptIsPassing = entity.AttemptIsPassing,
            AttemptNumber = entity.AttemptNumber,
            AttemptPinged = entity.AttemptPinged,
            AttemptPoints = entity.AttemptPoints,
            AttemptScore = entity.AttemptScore,
            AttemptStarted = entity.AttemptStarted,
            AttemptStatus = entity.AttemptStatus,
            AttemptTag = entity.AttemptTag,
            AttemptTimeLimit = entity.AttemptTimeLimit,
            AssessorUserId = entity.AssessorUserIdentifier,
            FormId = entity.FormIdentifier,
            FormPoints = entity.FormPoints,
            RegistrationId = entity.RegistrationIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            UserAgent = entity.UserAgent,
            AttemptSubmitted = entity.AttemptSubmitted,
            LearnerUserId = entity.LearnerUserIdentifier,
            GradingAssessorUserId = entity.GradingAssessorUserIdentifier,
            SectionsAsTabsEnabled = entity.SectionsAsTabsEnabled,
            TabNavigationEnabled = entity.TabNavigationEnabled,
            FormSectionsCount = entity.FormSectionsCount,
            ActiveSectionIndex = entity.ActiveSectionIndex,
            SingleQuestionPerTabEnabled = entity.SingleQuestionPerTabEnabled,
            ActiveQuestionIndex = entity.ActiveQuestionIndex,
            TabTimeLimit = entity.TabTimeLimit,
            AttemptPingInterval = entity.AttemptPingInterval,
            AttemptLanguage = entity.AttemptLanguage
        };

        if (entity.Assessment != null)
        {
            model.FormCode = entity.Assessment.FormCode;
            model.FormName = entity.Assessment.FormName;
        }

        return model;
    }

    public IEnumerable<AttemptMatch> ToMatch(IEnumerable<AttemptEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptMatch ToMatch(AttemptEntity entity)
    {
        var match = new AttemptMatch
        {
            AttemptId = entity.AttemptIdentifier

        };

        return match;
    }
}