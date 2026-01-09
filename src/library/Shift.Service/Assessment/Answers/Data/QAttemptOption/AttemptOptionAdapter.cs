using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptOptionAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptOption modify, AttemptOptionEntity entity)
    {
        entity.QuestionSequence = modify.QuestionSequence;
        entity.OptionPoints = modify.OptionPoints;
        entity.OptionSequence = modify.OptionSequence;
        entity.OptionText = modify.OptionText;
        entity.AnswerIsSelected = modify.AnswerIsSelected;
        entity.OptionCutScore = modify.OptionCutScore;
        entity.CompetencyItemIdentifier = modify.CompetencyItemIdentifier;
        entity.OptionIsTrue = modify.OptionIsTrue;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.OptionShape = modify.OptionShape;
        entity.OptionAnswerSequence = modify.OptionAnswerSequence;
        entity.CompetencyItemLabel = modify.CompetencyItemLabel;
        entity.CompetencyItemCode = modify.CompetencyItemCode;
        entity.CompetencyItemTitle = modify.CompetencyItemTitle;
        entity.CompetencyAreaLabel = modify.CompetencyAreaLabel;
        entity.CompetencyAreaCode = modify.CompetencyAreaCode;
        entity.CompetencyAreaTitle = modify.CompetencyAreaTitle;
        entity.CompetencyAreaIdentifier = modify.CompetencyAreaIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptOptionEntity ToEntity(CreateAttemptOption create)
    {
        var entity = new AttemptOptionEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionSequence = create.QuestionSequence,
            OptionKey = create.OptionKey,
            OptionPoints = create.OptionPoints,
            OptionSequence = create.OptionSequence,
            OptionText = create.OptionText,
            AnswerIsSelected = create.AnswerIsSelected,
            OptionCutScore = create.OptionCutScore,
            CompetencyItemIdentifier = create.CompetencyItemIdentifier,
            OptionIsTrue = create.OptionIsTrue,
            OrganizationIdentifier = create.OrganizationIdentifier,
            OptionShape = create.OptionShape,
            OptionAnswerSequence = create.OptionAnswerSequence,
            CompetencyItemLabel = create.CompetencyItemLabel,
            CompetencyItemCode = create.CompetencyItemCode,
            CompetencyItemTitle = create.CompetencyItemTitle,
            CompetencyAreaLabel = create.CompetencyAreaLabel,
            CompetencyAreaCode = create.CompetencyAreaCode,
            CompetencyAreaTitle = create.CompetencyAreaTitle,
            CompetencyAreaIdentifier = create.CompetencyAreaIdentifier
        };
        return entity;
    }

    public IEnumerable<AttemptOptionModel> ToModel(IEnumerable<AttemptOptionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptOptionModel ToModel(AttemptOptionEntity entity)
    {
        var model = new AttemptOptionModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionSequence = entity.QuestionSequence,
            OptionKey = entity.OptionKey,
            OptionPoints = entity.OptionPoints,
            OptionSequence = entity.OptionSequence,
            OptionText = entity.OptionText,
            AnswerIsSelected = entity.AnswerIsSelected,
            OptionCutScore = entity.OptionCutScore,
            CompetencyItemIdentifier = entity.CompetencyItemIdentifier,
            OptionIsTrue = entity.OptionIsTrue,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            OptionShape = entity.OptionShape,
            OptionAnswerSequence = entity.OptionAnswerSequence,
            CompetencyItemLabel = entity.CompetencyItemLabel,
            CompetencyItemCode = entity.CompetencyItemCode,
            CompetencyItemTitle = entity.CompetencyItemTitle,
            CompetencyAreaLabel = entity.CompetencyAreaLabel,
            CompetencyAreaCode = entity.CompetencyAreaCode,
            CompetencyAreaTitle = entity.CompetencyAreaTitle,
            CompetencyAreaIdentifier = entity.CompetencyAreaIdentifier
        };

        return model;
    }

    public IEnumerable<AttemptOptionMatch> ToMatch(IEnumerable<AttemptOptionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptOptionMatch ToMatch(AttemptOptionEntity entity)
    {
        var match = new AttemptOptionMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            OptionKey = entity.OptionKey,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}