using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptPinAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptPin modify, AttemptPinEntity entity)
    {
        entity.QuestionSequence = modify.QuestionSequence;
        entity.OptionKey = modify.OptionKey;
        entity.OptionPoints = modify.OptionPoints;
        entity.OptionSequence = modify.OptionSequence;
        entity.OptionText = modify.OptionText;
        entity.PinX = modify.PinX;
        entity.PinY = modify.PinY;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptPinEntity ToEntity(CreateAttemptPin create)
    {
        var entity = new AttemptPinEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionSequence = create.QuestionSequence,
            OptionKey = create.OptionKey,
            OptionPoints = create.OptionPoints,
            OptionSequence = create.OptionSequence,
            OptionText = create.OptionText,
            PinSequence = create.PinSequence,
            PinX = create.PinX,
            PinY = create.PinY
        };
        return entity;
    }

    public IEnumerable<AttemptPinModel> ToModel(IEnumerable<AttemptPinEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptPinModel ToModel(AttemptPinEntity entity)
    {
        var model = new AttemptPinModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionSequence = entity.QuestionSequence,
            OptionKey = entity.OptionKey,
            OptionPoints = entity.OptionPoints,
            OptionSequence = entity.OptionSequence,
            OptionText = entity.OptionText,
            PinSequence = entity.PinSequence,
            PinX = entity.PinX,
            PinY = entity.PinY
        };

        return model;
    }

    public IEnumerable<AttemptPinMatch> ToMatch(IEnumerable<AttemptPinEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptPinMatch ToMatch(AttemptPinEntity entity)
    {
        var match = new AttemptPinMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            PinSequence = entity.PinSequence,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}