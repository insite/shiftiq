using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Read
{
    public interface ISurveySearch
    {
        SurveyState GetSurveyState(Guid survey);

        string[] GetResponseAnswersText(Guid question);
        VSurveyResponseAnswer[] GetResponseAnswers(Guid question);
        VSurveyResponseAnswer[] GetResponseAnswers(Guid[] question);

        QResponseAnswer[] GetAnswersByResponse(Guid surveyFormIdentifier);
        QResponseOption[] GetOptionsByResponse(Guid surveyFormIdentifier);

        QSurveyForm GetSurveyForm(Guid surveyFormIdentifier, params Expression<Func<QSurveyForm, object>>[] includes);
        QSurveyForm GetSurveyFormByAsset(Guid organization, int form);
        QSurveyForm GetSurveyFormByName(Guid organization, string form);
        QSurveyForm GetSurveyFormByHook(Guid organization, string form);

        int CountSurveyForms(QSurveyFormFilter filter);
        List<QSurveyForm> GetSurveyForms(QSurveyFormFilter filter);

        List<QSurveyQuestion> GetSurveyQuestions(QSurveyQuestionFilter filter, params Expression<Func<QSurveyQuestion, object>>[] includes);
        QSurveyQuestion GetSurveyQuestion(Guid question, params Expression<Func<QSurveyQuestion, object>>[] includes);

        List<QSurveyOptionList> GetSurveyOptionLists(QSurveyOptionListFilter filter, params Expression<Func<QSurveyOptionList, object>>[] includes);
        QSurveyOptionList GetSurveyOptionList(Guid optionList, params Expression<Func<QSurveyOptionList, object>>[] includes);

        List<QSurveyOptionItem> GetSurveyOptionItems(QSurveyOptionItemFilter filter, params Expression<Func<QSurveyOptionItem, object>>[] includes);
        QSurveyOptionItem GetSurveyOptionItem(Guid option, params Expression<Func<QSurveyOptionItem, object>>[] includes);

        List<QSurveyCondition> GetSurveyConditions(QSurveyConditionFilter filter, params Expression<Func<QSurveyCondition, object>>[] includes);
        QSurveyCondition GetSurveyCondition(Guid optionItem, Guid question, params Expression<Func<QSurveyCondition, object>>[] includes);

        List<VUser> GetUsersWithMultiResponseSessions(Guid surveyForm);

        bool HasResponseOptions(Guid surveyOptionIdentifier);
        QResponseOption GetResponseOption(Guid responseSessionIdentifier, Guid surveyOptionIdentifier);

        QResponseSession[] GetOrphanResponses();
        int CountResponseAnswers(Guid surveyQuestionIdentifier);
        bool HasResponseSessions(QResponseSessionFilter filter);
        int CountResponseSessions(QResponseSessionFilter filter);
        int CountResponseSessions(QResponseAnalysisFilter filter);
        QResponseSession GetResponseSession(Guid session, params Expression<Func<QResponseSession, object>>[] includes);
        ISurveyResponse GetResponseSession(QResponseSessionFilter filter);

        List<ISurveyResponse> GetResponseSessions(Guid survey, Guid user);
        List<ISurveyResponse> GetResponseSessions(Guid survey, Guid[] users);
        List<ISurveyResponse> GetResponseSessions(Guid user);
        List<ISurveyResponse> GetResponseSessions(QResponseSessionFilter filter);

        List<ResponseSurveyUpload> GetResponseSurveyUploads(Guid organizationIdentifier, Guid respondentUserIdentifier, bool onlySurveyWithWorkflow = false);
        List<ResponseSurveyUpload> GetResponseSurveyUploads(Guid organizationIdentifier, Guid[] respondentUserIdentifiers, bool onlySurveyWithWorkflow = false);
        List<ResponseAnalysisSelectionItem> GetSelectionAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisChecklistItem> GetChecklistAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisCategoryItem> GetCategoryAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisIntegerItem> GetIntegerAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisTextItem> GetTextAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisCommentItem> GetCommentAnalysis(QResponseAnalysisFilter filter);
        List<ResponseAnalysisCorrelationItem> GetCorrelationAnalysis(Guid xAxisQuestionId, Guid yAxisQuestionId);

        bool IsDuplicate(SurveyForm survey);
        bool IsDuplicate(Guid organization, string name);
        bool IsValid(Guid survey);

        VSurveyResponseSummary GetSurveyResponseSummary(Guid survey);
        string FirstCommentAnswer(Guid session);
        string GetValueFromColumn(string id, string schema, string table, string column, string idColumn);
    }
}