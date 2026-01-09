using System;

using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;

namespace InSite.Application.Surveys.Read
{
    public interface ISurveyStore
    {
        void UpdateSurvey(SurveyBranchAdded e);
        void UpdateSurvey(SurveyBranchDeleted e);
        void UpdateSurvey(SurveyCommentDeleted e);
        void UpdateSurvey(SurveyCommentModified e);
        void UpdateSurvey(SurveyCommentPosted e);
        void UpdateSurvey(SurveyConditionAdded e);
        void UpdateSurvey(SurveyConditionDeleted e);
        void UpdateSurvey(SurveyFormAssetChanged e);
        void UpdateSurvey(SurveyFormContentChanged e);
        void InsertSurvey(SurveyFormCreated e);
        void UpdateSurvey(SurveyFormLanguagesChanged e);
        void UpdateSurvey(SurveyFormLocked e);
        void UpdateSurvey(SurveyFormMessageAdded e);
        void UpdateSurvey(SurveyFormMessagesChanged e);
        void UpdateSurvey(SurveyFormRenamed e);
        void UpdateSurvey(SurveyHookChanged e);
        void UpdateSurvey(SurveyFormScheduleChanged e);
        void UpdateSurvey(SurveyFormSettingsChanged e);
        void UpdateSurvey(SurveyDisplaySummaryChartChanged e);
        void UpdateSurvey(SurveyFormStatusChanged e);
        void UpdateSurvey(SurveyFormUnlocked e);
        void DeleteSurvey(SurveyFormDeleted e);
        void UpdateSurvey(SurveyOptionItemAdded e);
        void UpdateSurvey(SurveyOptionItemContentChanged e);
        void UpdateSurvey(SurveyOptionItemDeleted e);
        void UpdateSurvey(SurveyOptionItemSettingsChanged e);
        void UpdateSurvey(SurveyOptionItemsReordered e);
        void UpdateSurvey(SurveyOptionListAdded e);
        void UpdateSurvey(SurveyOptionListContentChanged e);
        void UpdateSurvey(SurveyOptionListDeleted e);
        void UpdateSurvey(SurveyOptionListsReordered e);
        void UpdateSurvey(SurveyQuestionAdded e);
        void UpdateSurvey(SurveyQuestionAttributed e);
        void UpdateSurvey(SurveyQuestionContentChanged e);
        void UpdateSurvey(SurveyQuestionRecoded e);
        void UpdateSurvey(SurveyQuestionDeleted e);
        void UpdateSurvey(SurveyQuestionSettingsChanged e);
        void UpdateSurvey(SurveyQuestionsReordered e);
        void UpdateSurvey(SurveyWorkflowConfigured e);

        void UpdateResponse(ResponseAnswerAdded e);
        void UpdateResponse(ResponseAnswerChanged e);
        void UpdateResponse(ResponseGroupChanged e);
        void UpdateResponse(ResponsePeriodChanged e);
        void UpdateResponse(ResponseUserChanged e);
        void UpdateResponse(ResponseOptionSelected e);
        void UpdateResponse(ResponseOptionUnselected e);
        void UpdateResponse(ResponseOptionsAdded e);
        void UpdateResponse(ResponseSessionCompleted e);
        void UpdateResponse(ResponseSessionConfirmed e);
        void InsertResponse(ResponseSessionCreated e);
        void UpdateResponse(ResponseSessionLocked e);
        void UpdateResponse(ResponseSessionReviewed e);
        void UpdateResponse(ResponseSessionStarted e);
        void UpdateResponse(ResponseSessionUnlocked e);

        void UpdateResponse(ResponseSessionFormConsent e);
        void DeleteResponse(ResponseSessionDeleted e);

        void DeleteAll();
        void DeleteAll(Guid id);
        void DeleteAllResponses();
        void DeleteAllResponses(Guid id);
    }
}
