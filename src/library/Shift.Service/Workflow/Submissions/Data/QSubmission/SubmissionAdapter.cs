using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionAdapter : IEntityAdapter
{
    public void Copy(ModifySubmission modify, SubmissionEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.SurveyFormIdentifier = modify.SurveyFormId;
        entity.ResponseSessionStatus = modify.ResponseSessionStatus;
        entity.RespondentUserIdentifier = modify.RespondentUserId;
        entity.RespondentLanguage = modify.RespondentLanguage;
        entity.ResponseIsLocked = modify.ResponseIsLocked;
        entity.ResponseSessionCreated = modify.ResponseSessionCreated;
        entity.ResponseSessionStarted = modify.ResponseSessionStarted;
        entity.ResponseSessionCompleted = modify.ResponseSessionCompleted;
        entity.GroupIdentifier = modify.GroupId;
        entity.PeriodIdentifier = modify.PeriodId;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.LastAnsweredQuestionIdentifier = modify.LastAnsweredQuestionId;

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
            OrganizationIdentifier = create.OrganizationId,
            SurveyFormIdentifier = create.SurveyFormId,
            ResponseSessionIdentifier = create.ResponseSessionId,
            ResponseSessionStatus = create.ResponseSessionStatus,
            RespondentUserIdentifier = create.RespondentUserId,
            RespondentLanguage = create.RespondentLanguage,
            ResponseIsLocked = create.ResponseIsLocked,
            ResponseSessionCreated = create.ResponseSessionCreated,
            ResponseSessionStarted = create.ResponseSessionStarted,
            ResponseSessionCompleted = create.ResponseSessionCompleted,
            GroupIdentifier = create.GroupId,
            PeriodIdentifier = create.PeriodId,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            LastAnsweredQuestionIdentifier = create.LastAnsweredQuestionId
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
            OrganizationId = entity.OrganizationIdentifier,
            SurveyFormId = entity.SurveyFormIdentifier,
            ResponseSessionId = entity.ResponseSessionIdentifier,
            ResponseSessionStatus = entity.ResponseSessionStatus,
            RespondentUserId = entity.RespondentUserIdentifier,
            RespondentLanguage = entity.RespondentLanguage,
            ResponseIsLocked = entity.ResponseIsLocked,
            ResponseSessionCreated = entity.ResponseSessionCreated,
            ResponseSessionStarted = entity.ResponseSessionStarted,
            ResponseSessionCompleted = entity.ResponseSessionCompleted,
            GroupId = entity.GroupIdentifier,
            PeriodId = entity.PeriodIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            LastAnsweredQuestionId = entity.LastAnsweredQuestionIdentifier
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
            ResponseSessionId = entity.ResponseSessionIdentifier

        };

        return match;
    }
}