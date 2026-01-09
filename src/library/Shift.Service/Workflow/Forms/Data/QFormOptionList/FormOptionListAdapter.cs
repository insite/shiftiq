using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormOptionListAdapter : IEntityAdapter
{
    public void Copy(ModifyFormOptionList modify, FormOptionListEntity entity)
    {
        entity.SurveyOptionListSequence = modify.SurveyOptionListSequence;
        entity.SurveyQuestionIdentifier = modify.SurveyQuestionIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FormOptionListEntity ToEntity(CreateFormOptionList create)
    {
        var entity = new FormOptionListEntity
        {
            SurveyOptionListIdentifier = create.SurveyOptionListIdentifier,
            SurveyOptionListSequence = create.SurveyOptionListSequence,
            SurveyQuestionIdentifier = create.SurveyQuestionIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<FormOptionListModel> ToModel(IEnumerable<FormOptionListEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FormOptionListModel ToModel(FormOptionListEntity entity)
    {
        var model = new FormOptionListModel
        {
            SurveyOptionListIdentifier = entity.SurveyOptionListIdentifier,
            SurveyOptionListSequence = entity.SurveyOptionListSequence,
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<FormOptionListMatch> ToMatch(IEnumerable<FormOptionListEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FormOptionListMatch ToMatch(FormOptionListEntity entity)
    {
        var match = new FormOptionListMatch
        {
            SurveyOptionListIdentifier = entity.SurveyOptionListIdentifier

        };

        return match;
    }
}