using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptQuestionAdapter : IEntityAdapter
{
    public void Copy(ModifyAttemptQuestion modify, AttemptQuestionEntity entity)
    {
        entity.QuestionPoints = modify.QuestionPoints;
        entity.QuestionSequence = modify.QuestionSequence;
        entity.QuestionText = modify.QuestionText;
        entity.AnswerOptionKey = modify.AnswerOptionKey;
        entity.AnswerOptionSequence = modify.AnswerOptionSequence;
        entity.AnswerPoints = modify.AnswerPoints;
        entity.CompetencyItemLabel = modify.CompetencyItemLabel;
        entity.CompetencyItemCode = modify.CompetencyItemCode;
        entity.CompetencyItemTitle = modify.CompetencyItemTitle;
        entity.CompetencyItemIdentifier = modify.CompetencyItemIdentifier;
        entity.CompetencyAreaLabel = modify.CompetencyAreaLabel;
        entity.CompetencyAreaCode = modify.CompetencyAreaCode;
        entity.CompetencyAreaTitle = modify.CompetencyAreaTitle;
        entity.CompetencyAreaIdentifier = modify.CompetencyAreaIdentifier;
        entity.QuestionType = modify.QuestionType;
        entity.AnswerText = modify.AnswerText;
        entity.QuestionCutScore = modify.QuestionCutScore;
        entity.QuestionMatchDistractors = modify.QuestionMatchDistractors;
        entity.QuestionCalculationMethod = modify.QuestionCalculationMethod;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.ParentQuestionIdentifier = modify.ParentQuestionIdentifier;
        entity.PinLimit = modify.PinLimit;
        entity.HotspotImage = modify.HotspotImage;
        entity.ShowShapes = modify.ShowShapes;
        entity.AnswerTimeLimit = modify.AnswerTimeLimit;
        entity.AnswerAttemptLimit = modify.AnswerAttemptLimit;
        entity.AnswerRequestAttempt = modify.AnswerRequestAttempt;
        entity.AnswerFileIdentifier = modify.AnswerFileIdentifier;
        entity.AnswerSolutionIdentifier = modify.AnswerSolutionIdentifier;
        entity.QuestionTopLabel = modify.QuestionTopLabel;
        entity.QuestionBottomLabel = modify.QuestionBottomLabel;
        entity.SectionIndex = modify.SectionIndex;
        entity.AnswerSubmitAttempt = modify.AnswerSubmitAttempt;
        entity.QuestionNumber = modify.QuestionNumber;
        entity.RubricRatingPoints = modify.RubricRatingPoints;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AttemptQuestionEntity ToEntity(CreateAttemptQuestion create)
    {
        var entity = new AttemptQuestionEntity
        {
            AttemptIdentifier = create.AttemptIdentifier,
            QuestionIdentifier = create.QuestionIdentifier,
            QuestionPoints = create.QuestionPoints,
            QuestionSequence = create.QuestionSequence,
            QuestionText = create.QuestionText,
            AnswerOptionKey = create.AnswerOptionKey,
            AnswerOptionSequence = create.AnswerOptionSequence,
            AnswerPoints = create.AnswerPoints,
            CompetencyItemLabel = create.CompetencyItemLabel,
            CompetencyItemCode = create.CompetencyItemCode,
            CompetencyItemTitle = create.CompetencyItemTitle,
            CompetencyItemIdentifier = create.CompetencyItemIdentifier,
            CompetencyAreaLabel = create.CompetencyAreaLabel,
            CompetencyAreaCode = create.CompetencyAreaCode,
            CompetencyAreaTitle = create.CompetencyAreaTitle,
            CompetencyAreaIdentifier = create.CompetencyAreaIdentifier,
            QuestionType = create.QuestionType,
            AnswerText = create.AnswerText,
            QuestionCutScore = create.QuestionCutScore,
            QuestionMatchDistractors = create.QuestionMatchDistractors,
            QuestionCalculationMethod = create.QuestionCalculationMethod,
            OrganizationIdentifier = create.OrganizationIdentifier,
            ParentQuestionIdentifier = create.ParentQuestionIdentifier,
            PinLimit = create.PinLimit,
            HotspotImage = create.HotspotImage,
            ShowShapes = create.ShowShapes,
            AnswerTimeLimit = create.AnswerTimeLimit,
            AnswerAttemptLimit = create.AnswerAttemptLimit,
            AnswerRequestAttempt = create.AnswerRequestAttempt,
            AnswerFileIdentifier = create.AnswerFileIdentifier,
            AnswerSolutionIdentifier = create.AnswerSolutionIdentifier,
            QuestionTopLabel = create.QuestionTopLabel,
            QuestionBottomLabel = create.QuestionBottomLabel,
            SectionIndex = create.SectionIndex,
            AnswerSubmitAttempt = create.AnswerSubmitAttempt,
            QuestionNumber = create.QuestionNumber,
            RubricRatingPoints = create.RubricRatingPoints
        };
        return entity;
    }

    public IEnumerable<AttemptQuestionModel> ToModel(IEnumerable<AttemptQuestionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AttemptQuestionModel ToModel(AttemptQuestionEntity entity)
    {
        var model = new AttemptQuestionModel
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier,
            QuestionPoints = entity.QuestionPoints,
            QuestionSequence = entity.QuestionSequence,
            QuestionText = entity.QuestionText,
            AnswerOptionKey = entity.AnswerOptionKey,
            AnswerOptionSequence = entity.AnswerOptionSequence,
            AnswerPoints = entity.AnswerPoints,
            CompetencyItemLabel = entity.CompetencyItemLabel,
            CompetencyItemCode = entity.CompetencyItemCode,
            CompetencyItemTitle = entity.CompetencyItemTitle,
            CompetencyItemIdentifier = entity.CompetencyItemIdentifier,
            CompetencyAreaLabel = entity.CompetencyAreaLabel,
            CompetencyAreaCode = entity.CompetencyAreaCode,
            CompetencyAreaTitle = entity.CompetencyAreaTitle,
            CompetencyAreaIdentifier = entity.CompetencyAreaIdentifier,
            QuestionType = entity.QuestionType,
            AnswerText = entity.AnswerText,
            QuestionCutScore = entity.QuestionCutScore,
            QuestionMatchDistractors = entity.QuestionMatchDistractors,
            QuestionCalculationMethod = entity.QuestionCalculationMethod,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            ParentQuestionIdentifier = entity.ParentQuestionIdentifier,
            PinLimit = entity.PinLimit,
            HotspotImage = entity.HotspotImage,
            ShowShapes = entity.ShowShapes,
            AnswerTimeLimit = entity.AnswerTimeLimit,
            AnswerAttemptLimit = entity.AnswerAttemptLimit,
            AnswerRequestAttempt = entity.AnswerRequestAttempt,
            AnswerFileIdentifier = entity.AnswerFileIdentifier,
            AnswerSolutionIdentifier = entity.AnswerSolutionIdentifier,
            QuestionTopLabel = entity.QuestionTopLabel,
            QuestionBottomLabel = entity.QuestionBottomLabel,
            SectionIndex = entity.SectionIndex,
            AnswerSubmitAttempt = entity.AnswerSubmitAttempt,
            QuestionNumber = entity.QuestionNumber,
            RubricRatingPoints = entity.RubricRatingPoints
        };

        return model;
    }

    public IEnumerable<AttemptQuestionMatch> ToMatch(IEnumerable<AttemptQuestionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AttemptQuestionMatch ToMatch(AttemptQuestionEntity entity)
    {
        var match = new AttemptQuestionMatch
        {
            AttemptIdentifier = entity.AttemptIdentifier,
            QuestionIdentifier = entity.QuestionIdentifier

        };

        return match;
    }
}