using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankFormAdapter : IEntityAdapter
{
    public void Copy(ModifyBankForm modify, BankFormEntity entity)
    {
        entity.BankIdentifier = modify.BankIdentifier;
        entity.FieldCount = modify.FieldCount;
        entity.FormAsset = modify.FormAsset;
        entity.FormName = modify.FormName;
        entity.FormPublicationStatus = modify.FormPublicationStatus;
        entity.FormTitle = modify.FormTitle;
        entity.FormType = modify.FormType;
        entity.FormVersion = modify.FormVersion;
        entity.SpecQuestionLimit = modify.SpecQuestionLimit;
        entity.SectionCount = modify.SectionCount;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.SpecIdentifier = modify.SpecIdentifier;
        entity.AttemptStartedCount = modify.AttemptStartedCount;
        entity.AttemptPassedCount = modify.AttemptPassedCount;
        entity.FormCode = modify.FormCode;
        entity.FormTimeLimit = modify.FormTimeLimit;
        entity.FormPassingScore = modify.FormPassingScore;
        entity.BankLevelType = modify.BankLevelType;
        entity.FormSource = modify.FormSource;
        entity.FormAssetVersion = modify.FormAssetVersion;
        entity.FormFirstPublished = modify.FormFirstPublished;
        entity.FormOrigin = modify.FormOrigin;
        entity.FormHook = modify.FormHook;
        entity.FormSummary = modify.FormSummary;
        entity.FormIntroduction = modify.FormIntroduction;
        entity.FormMaterialsForParticipation = modify.FormMaterialsForParticipation;
        entity.FormMaterialsForDistribution = modify.FormMaterialsForDistribution;
        entity.FormInstructionsForOnline = modify.FormInstructionsForOnline;
        entity.FormInstructionsForPaper = modify.FormInstructionsForPaper;
        entity.FormHasDiagrams = modify.FormHasDiagrams;
        entity.FormHasReferenceMaterials = modify.FormHasReferenceMaterials;
        entity.FormAttemptLimit = modify.FormAttemptLimit;
        entity.AttemptSubmittedCount = modify.AttemptSubmittedCount;
        entity.AttemptGradedCount = modify.AttemptGradedCount;
        entity.FormThirdPartyAssessmentIsEnabled = modify.FormThirdPartyAssessmentIsEnabled;
        entity.FormClassificationInstrument = modify.FormClassificationInstrument;
        entity.GradebookIdentifier = modify.GradebookIdentifier;
        entity.WhenAttemptStartedNotifyAdminMessageIdentifier = modify.WhenAttemptStartedNotifyAdminMessageIdentifier;
        entity.WhenAttemptCompletedNotifyAdminMessageIdentifier = modify.WhenAttemptCompletedNotifyAdminMessageIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankFormEntity ToEntity(CreateBankForm create)
    {
        var entity = new BankFormEntity
        {
            BankIdentifier = create.BankIdentifier,
            FieldCount = create.FieldCount,
            FormIdentifier = create.FormIdentifier,
            FormAsset = create.FormAsset,
            FormName = create.FormName,
            FormPublicationStatus = create.FormPublicationStatus,
            FormTitle = create.FormTitle,
            FormType = create.FormType,
            FormVersion = create.FormVersion,
            SpecQuestionLimit = create.SpecQuestionLimit,
            SectionCount = create.SectionCount,
            OrganizationIdentifier = create.OrganizationIdentifier,
            SpecIdentifier = create.SpecIdentifier,
            AttemptStartedCount = create.AttemptStartedCount,
            AttemptPassedCount = create.AttemptPassedCount,
            FormCode = create.FormCode,
            FormTimeLimit = create.FormTimeLimit,
            FormPassingScore = create.FormPassingScore,
            BankLevelType = create.BankLevelType,
            FormSource = create.FormSource,
            FormAssetVersion = create.FormAssetVersion,
            FormFirstPublished = create.FormFirstPublished,
            FormOrigin = create.FormOrigin,
            FormHook = create.FormHook,
            FormSummary = create.FormSummary,
            FormIntroduction = create.FormIntroduction,
            FormMaterialsForParticipation = create.FormMaterialsForParticipation,
            FormMaterialsForDistribution = create.FormMaterialsForDistribution,
            FormInstructionsForOnline = create.FormInstructionsForOnline,
            FormInstructionsForPaper = create.FormInstructionsForPaper,
            FormHasDiagrams = create.FormHasDiagrams,
            FormHasReferenceMaterials = create.FormHasReferenceMaterials,
            FormAttemptLimit = create.FormAttemptLimit,
            AttemptSubmittedCount = create.AttemptSubmittedCount,
            AttemptGradedCount = create.AttemptGradedCount,
            FormThirdPartyAssessmentIsEnabled = create.FormThirdPartyAssessmentIsEnabled,
            FormClassificationInstrument = create.FormClassificationInstrument,
            GradebookIdentifier = create.GradebookIdentifier,
            WhenAttemptStartedNotifyAdminMessageIdentifier = create.WhenAttemptStartedNotifyAdminMessageIdentifier,
            WhenAttemptCompletedNotifyAdminMessageIdentifier = create.WhenAttemptCompletedNotifyAdminMessageIdentifier
        };
        return entity;
    }

    public IEnumerable<BankFormModel> ToModel(IEnumerable<BankFormEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankFormModel ToModel(BankFormEntity entity)
    {
        var model = new BankFormModel
        {
            BankIdentifier = entity.BankIdentifier,
            FieldCount = entity.FieldCount,
            FormIdentifier = entity.FormIdentifier,
            FormAsset = entity.FormAsset,
            FormName = entity.FormName,
            FormPublicationStatus = entity.FormPublicationStatus,
            FormTitle = entity.FormTitle,
            FormType = entity.FormType,
            FormVersion = entity.FormVersion,
            SpecQuestionLimit = entity.SpecQuestionLimit,
            SectionCount = entity.SectionCount,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            SpecIdentifier = entity.SpecIdentifier,
            AttemptStartedCount = entity.AttemptStartedCount,
            AttemptPassedCount = entity.AttemptPassedCount,
            FormCode = entity.FormCode,
            FormTimeLimit = entity.FormTimeLimit,
            FormPassingScore = entity.FormPassingScore,
            BankLevelType = entity.BankLevelType,
            FormSource = entity.FormSource,
            FormAssetVersion = entity.FormAssetVersion,
            FormFirstPublished = entity.FormFirstPublished,
            FormOrigin = entity.FormOrigin,
            FormHook = entity.FormHook,
            FormSummary = entity.FormSummary,
            FormIntroduction = entity.FormIntroduction,
            FormMaterialsForParticipation = entity.FormMaterialsForParticipation,
            FormMaterialsForDistribution = entity.FormMaterialsForDistribution,
            FormInstructionsForOnline = entity.FormInstructionsForOnline,
            FormInstructionsForPaper = entity.FormInstructionsForPaper,
            FormHasDiagrams = entity.FormHasDiagrams,
            FormHasReferenceMaterials = entity.FormHasReferenceMaterials,
            FormAttemptLimit = entity.FormAttemptLimit,
            AttemptSubmittedCount = entity.AttemptSubmittedCount,
            AttemptGradedCount = entity.AttemptGradedCount,
            FormThirdPartyAssessmentIsEnabled = entity.FormThirdPartyAssessmentIsEnabled,
            FormClassificationInstrument = entity.FormClassificationInstrument,
            GradebookIdentifier = entity.GradebookIdentifier,
            WhenAttemptStartedNotifyAdminMessageIdentifier = entity.WhenAttemptStartedNotifyAdminMessageIdentifier,
            WhenAttemptCompletedNotifyAdminMessageIdentifier = entity.WhenAttemptCompletedNotifyAdminMessageIdentifier
        };

        return model;
    }

    public IEnumerable<BankFormMatch> ToMatch(IEnumerable<BankFormEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankFormMatch ToMatch(BankFormEntity entity)
    {
        var match = new BankFormMatch
        {
            FormIdentifier = entity.FormIdentifier

        };

        return match;
    }
}