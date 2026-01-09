using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormAdapter : IEntityAdapter
{
    public void Copy(ModifyForm modify, FormEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.SurveyFormStatus = modify.SurveyFormStatus;
        entity.SurveyFormLanguage = modify.SurveyFormLanguage;
        entity.SurveyFormLanguageTranslations = modify.SurveyFormLanguageTranslations;
        entity.SurveyFormName = modify.SurveyFormName;
        entity.UserFeedback = modify.UserFeedback;
        entity.RequireUserAuthentication = modify.RequireUserAuthentication;
        entity.RequireUserIdentification = modify.RequireUserIdentification;
        entity.AssetNumber = modify.AssetNumber;
        entity.ResponseLimitPerUser = modify.ResponseLimitPerUser;
        entity.SurveyFormOpened = modify.SurveyFormOpened;
        entity.SurveyFormClosed = modify.SurveyFormClosed;
        entity.SurveyMessageInvitation = modify.SurveyMessageInvitation;
        entity.SurveyMessageResponseStarted = modify.SurveyMessageResponseStarted;
        entity.SurveyMessageResponseCompleted = modify.SurveyMessageResponseCompleted;
        entity.SurveyMessageResponseConfirmed = modify.SurveyMessageResponseConfirmed;
        entity.SurveyFormLocked = modify.SurveyFormLocked;
        entity.SurveyFormDurationMinutes = modify.SurveyFormDurationMinutes;
        entity.SurveyFormHook = modify.SurveyFormHook;
        entity.PageCount = modify.PageCount;
        entity.QuestionCount = modify.QuestionCount;
        entity.BranchCount = modify.BranchCount;
        entity.ConditionCount = modify.ConditionCount;
        entity.EnableUserConfidentiality = modify.EnableUserConfidentiality;
        entity.DisplaySummaryChart = modify.DisplaySummaryChart;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.HasWorkflowConfiguration = modify.HasWorkflowConfiguration;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FormEntity ToEntity(CreateForm create)
    {
        var entity = new FormEntity
        {
            OrganizationIdentifier = create.OrganizationIdentifier,
            SurveyFormIdentifier = create.SurveyFormIdentifier,
            SurveyFormStatus = create.SurveyFormStatus,
            SurveyFormLanguage = create.SurveyFormLanguage,
            SurveyFormLanguageTranslations = create.SurveyFormLanguageTranslations,
            SurveyFormName = create.SurveyFormName,
            UserFeedback = create.UserFeedback,
            RequireUserAuthentication = create.RequireUserAuthentication,
            RequireUserIdentification = create.RequireUserIdentification,
            AssetNumber = create.AssetNumber,
            ResponseLimitPerUser = create.ResponseLimitPerUser,
            SurveyFormOpened = create.SurveyFormOpened,
            SurveyFormClosed = create.SurveyFormClosed,
            SurveyMessageInvitation = create.SurveyMessageInvitation,
            SurveyMessageResponseStarted = create.SurveyMessageResponseStarted,
            SurveyMessageResponseCompleted = create.SurveyMessageResponseCompleted,
            SurveyMessageResponseConfirmed = create.SurveyMessageResponseConfirmed,
            SurveyFormLocked = create.SurveyFormLocked,
            SurveyFormDurationMinutes = create.SurveyFormDurationMinutes,
            SurveyFormHook = create.SurveyFormHook,
            PageCount = create.PageCount,
            QuestionCount = create.QuestionCount,
            BranchCount = create.BranchCount,
            ConditionCount = create.ConditionCount,
            EnableUserConfidentiality = create.EnableUserConfidentiality,
            DisplaySummaryChart = create.DisplaySummaryChart,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            HasWorkflowConfiguration = create.HasWorkflowConfiguration
        };
        return entity;
    }

    public IEnumerable<FormModel> ToModel(IEnumerable<FormEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FormModel ToModel(FormEntity entity)
    {
        var model = new FormModel
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            SurveyFormIdentifier = entity.SurveyFormIdentifier,
            SurveyFormStatus = entity.SurveyFormStatus,
            SurveyFormLanguage = entity.SurveyFormLanguage,
            SurveyFormLanguageTranslations = entity.SurveyFormLanguageTranslations,
            SurveyFormName = entity.SurveyFormName,
            UserFeedback = entity.UserFeedback,
            RequireUserAuthentication = entity.RequireUserAuthentication,
            RequireUserIdentification = entity.RequireUserIdentification,
            AssetNumber = entity.AssetNumber,
            ResponseLimitPerUser = entity.ResponseLimitPerUser,
            SurveyFormOpened = entity.SurveyFormOpened,
            SurveyFormClosed = entity.SurveyFormClosed,
            SurveyMessageInvitation = entity.SurveyMessageInvitation,
            SurveyMessageResponseStarted = entity.SurveyMessageResponseStarted,
            SurveyMessageResponseCompleted = entity.SurveyMessageResponseCompleted,
            SurveyMessageResponseConfirmed = entity.SurveyMessageResponseConfirmed,
            SurveyFormLocked = entity.SurveyFormLocked,
            SurveyFormDurationMinutes = entity.SurveyFormDurationMinutes,
            SurveyFormHook = entity.SurveyFormHook,
            PageCount = entity.PageCount,
            QuestionCount = entity.QuestionCount,
            BranchCount = entity.BranchCount,
            ConditionCount = entity.ConditionCount,
            EnableUserConfidentiality = entity.EnableUserConfidentiality,
            DisplaySummaryChart = entity.DisplaySummaryChart,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            HasWorkflowConfiguration = entity.HasWorkflowConfiguration
        };

        return model;
    }

    public IEnumerable<FormMatch> ToMatch(IEnumerable<FormEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FormMatch ToMatch(FormEntity entity)
    {
        var match = new FormMatch
        {
            SurveyFormIdentifier = entity.SurveyFormIdentifier

        };

        return match;
    }
}