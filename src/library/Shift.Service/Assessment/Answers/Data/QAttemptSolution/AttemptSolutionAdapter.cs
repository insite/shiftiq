using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptSolutionAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptSolution modify, AttemptSolutionEntity entity)
    {
        entity.QuestionSequence = modify.QuestionSequence;
        entity.SolutionSequence = modify.SolutionSequence;
        entity.SolutionOptionsOrder = modify.SolutionOptionsOrder;
        entity.SolutionPoints = modify.SolutionPoints;
        entity.SolutionCutScore = modify.SolutionCutScore;
        entity.AnswerIsMatched = modify.AnswerIsMatched;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptSolutionEntity ToEntity(CreateAttemptSolution create)
    {
        var entity = new AttemptSolutionEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionSequence = create.QuestionSequence,
            SolutionIdentifier = create.SolutionIdentifier,
            SolutionSequence = create.SolutionSequence,
            SolutionOptionsOrder = create.SolutionOptionsOrder,
            SolutionPoints = create.SolutionPoints,
            SolutionCutScore = create.SolutionCutScore,
            AnswerIsMatched = create.AnswerIsMatched
        };
        return entity;
    }

    public IEnumerable<AttemptSolutionModel> ToModel(IEnumerable<AttemptSolutionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptSolutionModel ToModel(AttemptSolutionEntity entity)
    {
        var model = new AttemptSolutionModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionSequence = entity.QuestionSequence,
            SolutionIdentifier = entity.SolutionIdentifier,
            SolutionSequence = entity.SolutionSequence,
            SolutionOptionsOrder = entity.SolutionOptionsOrder,
            SolutionPoints = entity.SolutionPoints,
            SolutionCutScore = entity.SolutionCutScore,
            AnswerIsMatched = entity.AnswerIsMatched
        };

        return model;
    }

    public IEnumerable<AttemptSolutionMatch> ToMatch(IEnumerable<AttemptSolutionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptSolutionMatch ToMatch(AttemptSolutionEntity entity)
    {
        var match = new AttemptSolutionMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            SolutionIdentifier = entity.SolutionIdentifier

        };

        return match;
    }
}