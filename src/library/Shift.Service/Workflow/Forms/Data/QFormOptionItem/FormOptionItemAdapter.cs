using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormOptionItemAdapter : IEntityAdapter
{
    public void Copy(ModifyFormOptionItem modify, FormOptionItemEntity entity)
    {
        entity.SurveyOptionListIdentifier = modify.SurveyOptionListIdentifier;
        entity.SurveyOptionItemSequence = modify.SurveyOptionItemSequence;
        entity.BranchToQuestionIdentifier = modify.BranchToQuestionIdentifier;
        entity.SurveyOptionItemCategory = modify.SurveyOptionItemCategory;
        entity.SurveyOptionItemPoints = modify.SurveyOptionItemPoints;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FormOptionItemEntity ToEntity(CreateFormOptionItem create)
    {
        var entity = new FormOptionItemEntity
        {
            SurveyOptionListIdentifier = create.SurveyOptionListIdentifier,
            SurveyOptionItemIdentifier = create.SurveyOptionItemIdentifier,
            SurveyOptionItemSequence = create.SurveyOptionItemSequence,
            BranchToQuestionIdentifier = create.BranchToQuestionIdentifier,
            SurveyOptionItemCategory = create.SurveyOptionItemCategory,
            SurveyOptionItemPoints = create.SurveyOptionItemPoints,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<FormOptionItemModel> ToModel(IEnumerable<FormOptionItemEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FormOptionItemModel ToModel(FormOptionItemEntity entity)
    {
        var model = new FormOptionItemModel
        {
            SurveyOptionListIdentifier = entity.SurveyOptionListIdentifier,
            SurveyOptionItemIdentifier = entity.SurveyOptionItemIdentifier,
            SurveyOptionItemSequence = entity.SurveyOptionItemSequence,
            BranchToQuestionIdentifier = entity.BranchToQuestionIdentifier,
            SurveyOptionItemCategory = entity.SurveyOptionItemCategory,
            SurveyOptionItemPoints = entity.SurveyOptionItemPoints,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<FormOptionItemMatch> ToMatch(IEnumerable<FormOptionItemEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FormOptionItemMatch ToMatch(FormOptionItemEntity entity)
    {
        var match = new FormOptionItemMatch
        {
            SurveyOptionItemIdentifier = entity.SurveyOptionItemIdentifier

        };

        return match;
    }
}