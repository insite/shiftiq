using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptMatchAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptMatch modify, AttemptMatchEntity entity)
    {
        entity.QuestionSequence = modify.QuestionSequence;
        entity.MatchLeftText = modify.MatchLeftText;
        entity.MatchRightText = modify.MatchRightText;
        entity.AnswerText = modify.AnswerText;
        entity.MatchPoints = modify.MatchPoints;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptMatchEntity ToEntity(CreateAttemptMatch create)
    {
        var entity = new AttemptMatchEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionSequence = create.QuestionSequence,
            MatchSequence = create.MatchSequence,
            MatchLeftText = create.MatchLeftText,
            MatchRightText = create.MatchRightText,
            AnswerText = create.AnswerText,
            MatchPoints = create.MatchPoints,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<AttemptMatchModel> ToModel(IEnumerable<AttemptMatchEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptMatchModel ToModel(AttemptMatchEntity entity)
    {
        var model = new AttemptMatchModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionSequence = entity.QuestionSequence,
            MatchSequence = entity.MatchSequence,
            MatchLeftText = entity.MatchLeftText,
            MatchRightText = entity.MatchRightText,
            AnswerText = entity.AnswerText,
            MatchPoints = entity.MatchPoints,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<AttemptMatchMatch> ToMatch(IEnumerable<AttemptMatchEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptMatchMatch ToMatch(AttemptMatchEntity entity)
    {
        var match = new AttemptMatchMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            MatchSequence = entity.MatchSequence,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}