using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class QuizAttemptAdapter : IEntityAdapter
{
    public void Copy(ModifyQuizAttempt modify, QuizAttemptEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.QuizIdentifier = modify.QuizIdentifier;
        entity.LearnerIdentifier = modify.LearnerIdentifier;
        entity.AttemptCreated = modify.AttemptCreated;
        entity.AttemptStarted = modify.AttemptStarted;
        entity.AttemptCompleted = modify.AttemptCompleted;
        entity.QuizGradebookIdentifier = modify.QuizGradebookIdentifier;
        entity.QuizType = modify.QuizType;
        entity.QuizName = modify.QuizName;
        entity.QuizData = modify.QuizData;
        entity.QuizTimeLimit = modify.QuizTimeLimit;
        entity.QuizPassingAccuracy = modify.QuizPassingAccuracy;
        entity.QuizPassingWpm = modify.QuizPassingWpm;
        entity.QuizPassingKph = modify.QuizPassingKph;
        entity.AttemptData = modify.AttemptData;
        entity.AttemptIsPassing = modify.AttemptIsPassing;
        entity.AttemptScore = modify.AttemptScore;
        entity.AttemptDuration = modify.AttemptDuration;
        entity.AttemptMistakes = modify.AttemptMistakes;
        entity.AttemptAccuracy = modify.AttemptAccuracy;
        entity.AttemptCharsPerMin = modify.AttemptCharsPerMin;
        entity.AttemptWordsPerMin = modify.AttemptWordsPerMin;
        entity.AttemptKeystrokesPerHour = modify.AttemptKeystrokesPerHour;
        entity.AttemptSpeed = modify.AttemptSpeed;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public QuizAttemptEntity ToEntity(CreateQuizAttempt create)
    {
        var entity = new QuizAttemptEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            QuizIdentifier = create.QuizIdentifier,
            LearnerIdentifier = create.LearnerIdentifier,
            AttemptCreated = create.AttemptCreated,
            AttemptStarted = create.AttemptStarted,
            AttemptCompleted = create.AttemptCompleted,
            QuizGradebookIdentifier = create.QuizGradebookIdentifier,
            QuizType = create.QuizType,
            QuizName = create.QuizName,
            QuizData = create.QuizData,
            QuizTimeLimit = create.QuizTimeLimit,
            QuizPassingAccuracy = create.QuizPassingAccuracy,
            QuizPassingWpm = create.QuizPassingWpm,
            QuizPassingKph = create.QuizPassingKph,
            AttemptData = create.AttemptData,
            AttemptIsPassing = create.AttemptIsPassing,
            AttemptScore = create.AttemptScore,
            AttemptDuration = create.AttemptDuration,
            AttemptMistakes = create.AttemptMistakes,
            AttemptAccuracy = create.AttemptAccuracy,
            AttemptCharsPerMin = create.AttemptCharsPerMin,
            AttemptWordsPerMin = create.AttemptWordsPerMin,
            AttemptKeystrokesPerHour = create.AttemptKeystrokesPerHour,
            AttemptSpeed = create.AttemptSpeed
        };
        return entity;
    }

    public IEnumerable<QuizAttemptModel> ToModel(IEnumerable<QuizAttemptEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public QuizAttemptModel ToModel(QuizAttemptEntity entity)
    {
        var model = new QuizAttemptModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            QuizIdentifier = entity.QuizIdentifier,
            LearnerIdentifier = entity.LearnerIdentifier,
            AttemptCreated = entity.AttemptCreated,
            AttemptStarted = entity.AttemptStarted,
            AttemptCompleted = entity.AttemptCompleted,
            QuizGradebookIdentifier = entity.QuizGradebookIdentifier,
            QuizType = entity.QuizType,
            QuizName = entity.QuizName,
            QuizData = entity.QuizData,
            QuizTimeLimit = entity.QuizTimeLimit,
            QuizPassingAccuracy = entity.QuizPassingAccuracy,
            QuizPassingWpm = entity.QuizPassingWpm,
            QuizPassingKph = entity.QuizPassingKph,
            AttemptData = entity.AttemptData,
            AttemptIsPassing = entity.AttemptIsPassing,
            AttemptScore = entity.AttemptScore,
            AttemptDuration = entity.AttemptDuration,
            AttemptMistakes = entity.AttemptMistakes,
            AttemptAccuracy = entity.AttemptAccuracy,
            AttemptCharsPerMin = entity.AttemptCharsPerMin,
            AttemptWordsPerMin = entity.AttemptWordsPerMin,
            AttemptKeystrokesPerHour = entity.AttemptKeystrokesPerHour,
            AttemptSpeed = entity.AttemptSpeed
        };

        return model;
    }

    public IEnumerable<QuizAttemptMatch> ToMatch(IEnumerable<QuizAttemptEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public QuizAttemptMatch ToMatch(QuizAttemptEntity entity)
    {
        var match = new QuizAttemptMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier

        };

        return match;
    }
}