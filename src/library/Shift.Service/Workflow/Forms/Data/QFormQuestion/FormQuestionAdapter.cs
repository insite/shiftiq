using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormQuestionAdapter : IEntityAdapter
{
    public void Copy(ModifyFormQuestion modify, FormQuestionEntity entity)
    {
        entity.SurveyFormIdentifier = modify.SurveyFormIdentifier;
        entity.SurveyQuestionType = modify.SurveyQuestionType;
        entity.SurveyQuestionCode = modify.SurveyQuestionCode;
        entity.SurveyQuestionIndicator = modify.SurveyQuestionIndicator;
        entity.SurveyQuestionSequence = modify.SurveyQuestionSequence;
        entity.SurveyQuestionIsRequired = modify.SurveyQuestionIsRequired;
        entity.SurveyQuestionListEnableBranch = modify.SurveyQuestionListEnableBranch;
        entity.SurveyQuestionListEnableOtherText = modify.SurveyQuestionListEnableOtherText;
        entity.SurveyQuestionListEnableRandomization = modify.SurveyQuestionListEnableRandomization;
        entity.SurveyQuestionNumberEnableStatistics = modify.SurveyQuestionNumberEnableStatistics;
        entity.SurveyQuestionTextCharacterLimit = modify.SurveyQuestionTextCharacterLimit;
        entity.SurveyQuestionTextLineCount = modify.SurveyQuestionTextLineCount;
        entity.SurveyQuestionIsNested = modify.SurveyQuestionIsNested;
        entity.SurveyQuestionSource = modify.SurveyQuestionSource;
        entity.SurveyQuestionAttribute = modify.SurveyQuestionAttribute;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.SurveyQuestionListEnableGroupMembership = modify.SurveyQuestionListEnableGroupMembership;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FormQuestionEntity ToEntity(CreateFormQuestion create)
    {
        var entity = new FormQuestionEntity
        {
            SurveyQuestionIdentifier = create.SurveyQuestionIdentifier,
            SurveyFormIdentifier = create.SurveyFormIdentifier,
            SurveyQuestionType = create.SurveyQuestionType,
            SurveyQuestionCode = create.SurveyQuestionCode,
            SurveyQuestionIndicator = create.SurveyQuestionIndicator,
            SurveyQuestionSequence = create.SurveyQuestionSequence,
            SurveyQuestionIsRequired = create.SurveyQuestionIsRequired,
            SurveyQuestionListEnableBranch = create.SurveyQuestionListEnableBranch,
            SurveyQuestionListEnableOtherText = create.SurveyQuestionListEnableOtherText,
            SurveyQuestionListEnableRandomization = create.SurveyQuestionListEnableRandomization,
            SurveyQuestionNumberEnableStatistics = create.SurveyQuestionNumberEnableStatistics,
            SurveyQuestionTextCharacterLimit = create.SurveyQuestionTextCharacterLimit,
            SurveyQuestionTextLineCount = create.SurveyQuestionTextLineCount,
            SurveyQuestionIsNested = create.SurveyQuestionIsNested,
            SurveyQuestionSource = create.SurveyQuestionSource,
            SurveyQuestionAttribute = create.SurveyQuestionAttribute,
            OrganizationIdentifier = create.OrganizationIdentifier,
            SurveyQuestionListEnableGroupMembership = create.SurveyQuestionListEnableGroupMembership
        };
        return entity;
    }

    public IEnumerable<FormQuestionModel> ToModel(IEnumerable<FormQuestionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FormQuestionModel ToModel(FormQuestionEntity entity)
    {
        var model = new FormQuestionModel
        {
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier,
            SurveyFormIdentifier = entity.SurveyFormIdentifier,
            SurveyQuestionType = entity.SurveyQuestionType,
            SurveyQuestionCode = entity.SurveyQuestionCode,
            SurveyQuestionIndicator = entity.SurveyQuestionIndicator,
            SurveyQuestionSequence = entity.SurveyQuestionSequence,
            SurveyQuestionIsRequired = entity.SurveyQuestionIsRequired,
            SurveyQuestionListEnableBranch = entity.SurveyQuestionListEnableBranch,
            SurveyQuestionListEnableOtherText = entity.SurveyQuestionListEnableOtherText,
            SurveyQuestionListEnableRandomization = entity.SurveyQuestionListEnableRandomization,
            SurveyQuestionNumberEnableStatistics = entity.SurveyQuestionNumberEnableStatistics,
            SurveyQuestionTextCharacterLimit = entity.SurveyQuestionTextCharacterLimit,
            SurveyQuestionTextLineCount = entity.SurveyQuestionTextLineCount,
            SurveyQuestionIsNested = entity.SurveyQuestionIsNested,
            SurveyQuestionSource = entity.SurveyQuestionSource,
            SurveyQuestionAttribute = entity.SurveyQuestionAttribute,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            SurveyQuestionListEnableGroupMembership = entity.SurveyQuestionListEnableGroupMembership
        };

        return model;
    }

    public IEnumerable<FormQuestionMatch> ToMatch(IEnumerable<FormQuestionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FormQuestionMatch ToMatch(FormQuestionEntity entity)
    {
        var match = new FormQuestionMatch
        {
            SurveyQuestionIdentifier = entity.SurveyQuestionIdentifier

        };

        return match;
    }
}