using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionAnswerAdapter : IEntityAdapter
{
    public void Copy(ModifySubmissionAnswer modify, SubmissionAnswerEntity entity)
    {
        entity.RespondentUserIdentifier = modify.RespondentUserIdentifier;
        entity.ResponseAnswerText = modify.ResponseAnswerText;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public SubmissionAnswerEntity ToEntity(CreateSubmissionAnswer create)
    {
        var entity = new SubmissionAnswerEntity
        {
            ResponseSessionIdentifier = create.ResponseSessionIdentifier,
            SurveyQuestionIdentifier = create.SurveyQuestionIdentifier,
            RespondentUserIdentifier = create.RespondentUserIdentifier,
            ResponseAnswerText = create.ResponseAnswerText,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<SubmissionAnswerModel> ToModel(IEnumerable<SubmissionAnswerEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public SubmissionAnswerModel ToModel(SubmissionAnswerEntity entity)
    {
        var model = new SubmissionAnswerModel
        {
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier,
            RespondentUserIdentifier = entity.RespondentUserIdentifier,
            ResponseAnswerText = entity.ResponseAnswerText,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<SubmissionAnswerMatch> ToMatch(IEnumerable<SubmissionAnswerEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public SubmissionAnswerMatch ToMatch(SubmissionAnswerEntity entity)
    {
        var match = new SubmissionAnswerMatch
        {
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier

        };

        return match;
    }
}