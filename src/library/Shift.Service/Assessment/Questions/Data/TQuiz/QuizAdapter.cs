using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class QuizAdapter : IEntityAdapter
{
    public void Copy(ModifyQuiz modify, QuizEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.GradebookIdentifier = modify.GradebookIdentifier;
        entity.QuizType = modify.QuizType;
        entity.QuizName = modify.QuizName;
        entity.QuizData = modify.QuizData;
        entity.TimeLimit = modify.TimeLimit;
        entity.AttemptLimit = modify.AttemptLimit;
        entity.PassingScore = modify.PassingScore;
        entity.MaximumPoints = modify.MaximumPoints;
        entity.PassingPoints = modify.PassingPoints;
        entity.PassingAccuracy = modify.PassingAccuracy;
        entity.PassingWpm = modify.PassingWpm;
        entity.PassingKph = modify.PassingKph;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public QuizEntity ToEntity(CreateQuiz create)
    {
        var entity = new QuizEntity
        {
            QuizIdentifier = create.QuizIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            GradebookIdentifier = create.GradebookIdentifier,
            QuizType = create.QuizType,
            QuizName = create.QuizName,
            QuizData = create.QuizData,
            TimeLimit = create.TimeLimit,
            AttemptLimit = create.AttemptLimit,
            PassingScore = create.PassingScore,
            MaximumPoints = create.MaximumPoints,
            PassingPoints = create.PassingPoints,
            PassingAccuracy = create.PassingAccuracy,
            PassingWpm = create.PassingWpm,
            PassingKph = create.PassingKph
        };
        return entity;
    }

    public IEnumerable<QuizModel> ToModel(IEnumerable<QuizEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public QuizModel ToModel(QuizEntity entity)
    {
        var model = new QuizModel
        {
            QuizIdentifier = entity.QuizIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            GradebookIdentifier = entity.GradebookIdentifier,
            QuizType = entity.QuizType,
            QuizName = entity.QuizName,
            QuizData = entity.QuizData,
            TimeLimit = entity.TimeLimit,
            AttemptLimit = entity.AttemptLimit,
            PassingScore = entity.PassingScore,
            MaximumPoints = entity.MaximumPoints,
            PassingPoints = entity.PassingPoints,
            PassingAccuracy = entity.PassingAccuracy,
            PassingWpm = entity.PassingWpm,
            PassingKph = entity.PassingKph
        };

        return model;
    }

    public IEnumerable<QuizMatch> ToMatch(IEnumerable<QuizEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public QuizMatch ToMatch(QuizEntity entity)
    {
        var match = new QuizMatch
        {
            QuizIdentifier = entity.QuizIdentifier

        };

        return match;
    }
}