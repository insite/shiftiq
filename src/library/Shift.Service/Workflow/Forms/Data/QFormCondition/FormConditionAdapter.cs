using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormConditionAdapter : IEntityAdapter
{
    public void Copy(ModifyFormCondition modify, FormConditionEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FormConditionEntity ToEntity(CreateFormCondition create)
    {
        var entity = new FormConditionEntity
        {
            MaskingSurveyOptionItemIdentifier = create.MaskingSurveyOptionItemIdentifier,
            MaskedSurveyQuestionIdentifier = create.MaskedSurveyQuestionIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<FormConditionModel> ToModel(IEnumerable<FormConditionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FormConditionModel ToModel(FormConditionEntity entity)
    {
        var model = new FormConditionModel
        {
            MaskingSurveyOptionItemIdentifier = entity.MaskingSurveyOptionItemIdentifier,
            MaskedSurveyQuestionIdentifier = entity.MaskedSurveyQuestionIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<FormConditionMatch> ToMatch(IEnumerable<FormConditionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FormConditionMatch ToMatch(FormConditionEntity entity)
    {
        var match = new FormConditionMatch
        {
            MaskedSurveyQuestionIdentifier = entity.MaskedSurveyQuestionIdentifier,
            MaskingSurveyOptionItemIdentifier = entity.MaskingSurveyOptionItemIdentifier

        };

        return match;
    }
}