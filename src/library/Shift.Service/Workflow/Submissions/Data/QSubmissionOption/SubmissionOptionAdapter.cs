using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionOptionAdapter : IEntityAdapter
{
    public void Copy(ModifySubmissionOption modify, SubmissionOptionEntity entity)
    {
        entity.ResponseOptionIsSelected = modify.ResponseOptionIsSelected;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.SurveyQuestionIdentifier = modify.SurveyQuestionIdentifier;
        entity.OptionSequence = modify.OptionSequence;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public SubmissionOptionEntity ToEntity(CreateSubmissionOption create)
    {
        var entity = new SubmissionOptionEntity
        {
            ResponseSessionIdentifier = create.ResponseSessionIdentifier,
            SurveyOptionIdentifier = create.SurveyOptionIdentifier,
            ResponseOptionIsSelected = create.ResponseOptionIsSelected,
            OrganizationIdentifier = create.OrganizationIdentifier,
            SurveyQuestionIdentifier = create.SurveyQuestionIdentifier,
            OptionSequence = create.OptionSequence
        };
        return entity;
    }

    public IEnumerable<SubmissionOptionModel> ToModel(IEnumerable<SubmissionOptionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public SubmissionOptionModel ToModel(SubmissionOptionEntity entity)
    {
        var model = new SubmissionOptionModel
        {
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
            SurveyOptionIdentifier = entity.SurveyOptionIdentifier,
            ResponseOptionIsSelected = entity.ResponseOptionIsSelected,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier,
            OptionSequence = entity.OptionSequence
        };

        return model;
    }

    public IEnumerable<SubmissionOptionMatch> ToMatch(IEnumerable<SubmissionOptionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public SubmissionOptionMatch ToMatch(SubmissionOptionEntity entity)
    {
        var match = new SubmissionOptionMatch
        {
            ResponseSessionIdentifier = entity.ResponseSessionIdentifier,
            SurveyOptionIdentifier = entity.SurveyOptionIdentifier

        };

        return match;
    }
}