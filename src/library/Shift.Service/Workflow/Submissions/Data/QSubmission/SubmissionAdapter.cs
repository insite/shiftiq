using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionAdapter : IEntityAdapter
{
    public void Copy(ModifySubmission modify, SubmissionEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.SurveyFormIdentifier = modify.SurveyFormIdentifier;
        entity.ResponseSessionStatus = modify.ResponseSessionStatus;
        entity.RespondentUserIdentifier = modify.RespondentUserIdentifier;
        entity.RespondentLanguage = modify.RespondentLanguage;
        entity.ResponseIsLocked = modify.ResponseIsLocked;
        entity.ResponseSessionCreated = modify.ResponseSessionCreated;
        entity.ResponseSessionStarted = modify.ResponseSessionStarted;
        entity.ResponseSessionCompleted = modify.ResponseSessionCompleted;
        entity.GroupIdentifier = modify.GroupIdentifier;
        entity.PeriodIdentifier = modify.PeriodIdentifier;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.LastAnsweredQuestionIdentifier = modify.LastAnsweredQuestionIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public SubmissionEntity ToEntity(CreateSubmission create)
    {
        var entity = new SubmissionEntity
        {
            OrganizationIdentifier = create.OrganizationIdentifier,
            SurveyFormIdentifier = create.SurveyFormIdentifier,
            ResponseSessionIdentifier = create.ResponseSessionIdentifier,
            ResponseSessionStatus = create.ResponseSessionStatus,
            RespondentUserIdentifier = create.RespondentUserIdentifier,
            RespondentLanguage = create.RespondentLanguage,
            ResponseIsLocked = create.ResponseIsLocked,
            ResponseSessionCreated = create.ResponseSessionCreated,
            ResponseSessionStarted = create.ResponseSessionStarted,
            ResponseSessionCompleted = create.ResponseSessionCompleted,
            GroupIdentifier = create.GroupIdentifier,
            PeriodIdentifier = create.PeriodIdentifier,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            LastAnsweredQuestionIdentifier = create.LastAnsweredQuestionIdentifier
        };
        return entity;
    }

    public IEnumerable<SubmissionModel> ToModel(IEnumerable<SubmissionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public SubmissionModel ToModel(SubmissionEntity entity)
    {
        var model = new SubmissionModel
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            SurveyFormIdentifier = entity.SurveyFormIdentifier,
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
            ResponseSessionStatus = entity.ResponseSessionStatus,
            RespondentUserIdentifier = entity.RespondentUserIdentifier,
            RespondentLanguage = entity.RespondentLanguage,
            ResponseIsLocked = entity.ResponseIsLocked,
            ResponseSessionCreated = entity.ResponseSessionCreated,
            ResponseSessionStarted = entity.ResponseSessionStarted,
            ResponseSessionCompleted = entity.ResponseSessionCompleted,
            GroupIdentifier = entity.GroupIdentifier,
            PeriodIdentifier = entity.PeriodIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            LastAnsweredQuestionIdentifier = entity.LastAnsweredQuestionIdentifier
        };

        return model;
    }

    public IEnumerable<SubmissionMatch> ToMatch(IEnumerable<SubmissionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public SubmissionMatch ToMatch(SubmissionEntity entity)
    {
        var match = new SubmissionMatch
        {
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier

        };

        return match;
    }
}