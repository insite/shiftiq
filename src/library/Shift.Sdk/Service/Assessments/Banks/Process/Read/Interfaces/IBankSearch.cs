using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;
using InSite.Application.Logs.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Read
{
    public interface IBankSearch
    {
        BankEntityType GetEntityType(Guid id);

        int CountQuestions(QBankQuestionFilter filter);
        QBankQuestion GetQuestion(Guid question);
        Question GetQuestionData(Guid id);
        List<QBankQuestion> GetQuestions(IEnumerable<Guid> ids, params Expression<Func<QBankQuestion, object>>[] includes);
        List<QBankQuestion> GetQuestions(QBankQuestionFilter filter, params Expression<Func<QBankQuestion, object>>[] includes);
        List<QBankQuestionDetail> GetQuestionDetails(QBankQuestionFilter filter);
        List<QBankQuestionGradeItem> GetQuestionGradeItems(IEnumerable<Guid> questionIds);
        List<Guid> GetQuestionsNotConnectedToRubrics(IEnumerable<Guid> questionIds);

        int CountBanks(QBankFilter filter);
        List<Counter> CountBanksByType(QBankFilter filter);
        QBank GetBank(Guid id);
        BankState GetBankState(Guid bank);
        BankState[] GetBankStates(IEnumerable<Guid> banks);
        QBank[] GetBanks(IEnumerable<Guid> id);
        List<QBank> GetBanks(QBankFilter filter);
        int CountBankOccupations(Guid organizationId, string searchText);
        BankSummaryOccupationInfo[] GetBankOccupations(Guid organizationId, Guid[] occupationIds);
        BankSummaryOccupationInfo[] GetBankOccupations(Guid organizationId, Paging paging, string searchText);
        List<string> GetBankLevels(QBankFilter filter);

        BankSummaryOccupationInfo GetBankOccupation(Guid organizationId, Guid id);
        int CountBankFrameworks(Guid organizationId, Guid? occupationId, string searchText);
        BankSummaryFrameworkInfo[] GetBankFrameworks(Guid organizationId, Guid[] frameworkIds);
        BankSummaryFrameworkInfo[] GetBankFrameworks(Guid organizationId, Guid? occupationId, Paging paging, string searchText);
        BankSummaryFrameworkInfo GetBankFramework(Guid organizationId, Guid id);
        List<Guid> GetBanksWithDuplicateFormAsset();

        MostRecentChange[] GetMostRecentlyChangedBanks(Guid organization, int count, string additionalWhere = null);

        int CountForms(QBankFormFilter filter);
        Form GetFormData(Guid id);
        QBankForm GetForm(Guid id);
        List<QBankForm> GetForms(IEnumerable<Guid> id, params Expression<Func<QBankForm, object>>[] includes);

        QBankForm[] GetForms(QBankFormFilter filter, params Expression<Func<QBankForm, object>>[] includes);
        QBankForm[] GetForms(QBankFormFilter filter);

        int Count(QBankSpecificationFilter filter);
        QBankSpecification[] Get(QBankSpecificationFilter filter);
        QBankSpecification GetSpecification(Guid id);

        VComment GetComment(Guid comment);
        VComment[] GetComments(Guid bank);
        int CountComments(BankCommentaryFilter filter);
        T[] BindComments<T>(Expression<Func<VComment, T>> binder, BankCommentaryFilter filter);

        VAssessmentFormRegistration[] GetAssessmentFormRegistrations(Guid? @event);
    }
}