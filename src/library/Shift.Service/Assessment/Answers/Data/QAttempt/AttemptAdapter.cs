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
        entity.AssessorUserIdentifier = modify.AssessorUserIdentifier;
        entity.FormIdentifier = modify.FormIdentifier;
        entity.FormPoints = modify.FormPoints;
        entity.RegistrationIdentifier = modify.RegistrationIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.UserAgent = modify.UserAgent;
        entity.AttemptSubmitted = modify.AttemptSubmitted;
        entity.LearnerUserIdentifier = modify.LearnerUserIdentifier;
        entity.GradingAssessorUserIdentifier = modify.GradingAssessorUserIdentifier;
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
            AttemptIdentifier = create.AttemptIdentifier,
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
            AssessorUserIdentifier = create.AssessorUserIdentifier,
            FormIdentifier = create.FormIdentifier,
            FormPoints = create.FormPoints,
            RegistrationIdentifier = create.RegistrationIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            UserAgent = create.UserAgent,
            AttemptSubmitted = create.AttemptSubmitted,
            LearnerUserIdentifier = create.LearnerUserIdentifier,
            GradingAssessorUserIdentifier = create.GradingAssessorUserIdentifier,
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
            AttemptIdentifier = entity.AttemptIdentifier,
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
            AssessorUserIdentifier = entity.AssessorUserIdentifier,
            FormIdentifier = entity.FormIdentifier,
            FormPoints = entity.FormPoints,
            RegistrationIdentifier = entity.RegistrationIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            UserAgent = entity.UserAgent,
            AttemptSubmitted = entity.AttemptSubmitted,
            LearnerUserIdentifier = entity.LearnerUserIdentifier,
            GradingAssessorUserIdentifier = entity.GradingAssessorUserIdentifier,
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
            AttemptIdentifier = entity.AttemptIdentifier

        };

        return match;
    }
}