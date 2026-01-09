using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Application.Logs.Read;
using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class SurveySearch : ISurveySearch
    {
        private readonly IAggregateSearch _aggregateSearch;

        public SurveySearch(IAggregateSearch aggregateSearch)
        {
            _aggregateSearch = aggregateSearch;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public static Guid[] TempSelectQuestionsThatNeedToEnableOtherText()
        {
            const string select = @"SELECT q.SurveyQuestionIdentifier
FROM surveys.QSurveyQuestion AS q
INNER JOIN surveys.QSurveyOptionList AS l ON l.SurveyQuestionIdentifier = q.SurveyQuestionIdentifier
INNER JOIN surveys.QSurveyOptionItem AS i ON i.SurveyOptionListIdentifier = l.SurveyOptionListIdentifier
INNER JOIN contents.TContent AS c ON i.SurveyOptionItemIdentifier = c.ContainerIdentifier AND c.ContentLabel = 'Title' AND c.ContentLanguage = 'en'
INNER JOIN surveys.QSurveyForm AS f ON f.SurveyFormIdentifier = q.SurveyFormIdentifier
INNER JOIN accounts.QOrganization AS t ON t.OrganizationIdentifier = f.OrganizationIdentifier
INNER JOIN contents.TContent x ON x.ContainerIdentifier = q.SurveyQuestionIdentifier AND x.ContentLanguage = c.ContentLanguage 
WHERE c.ContentText = 'Other - please specify'
AND q.SurveyQuestionListEnableOtherText = 0
AND q.SurveyQuestionType = 'RadioList'";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Guid>(select).ToArray();
        }

        public List<ResponseSurveyUpload> GetResponseSurveyUploads(Guid organizationIdentifier, Guid respondentUserIdentifier, bool onlySurveyWithWorkflow = false)
        {
            return GetResponseSurveyUploads(organizationIdentifier, new[] { respondentUserIdentifier }, onlySurveyWithWorkflow);
        }

        public List<ResponseSurveyUpload> GetResponseSurveyUploads(Guid organizationIdentifier, Guid[] respondentUserIdentifiers, bool onlySurveyWithWorkflow = false)
        {
            using (var db = new InternalDbContext(false))
            {
                var query = db.QResponseAnswers
                    .Where(x =>
                        x.ResponseSession.OrganizationIdentifier == organizationIdentifier
                        && respondentUserIdentifiers.Any(r => r == x.ResponseSession.RespondentUserIdentifier)
                        && x.SurveyQuestion.SurveyQuestionType == "Upload"
                    );

                if (onlySurveyWithWorkflow)
                    query = query.Where(x => x.ResponseSession.SurveyForm.HasWorkflowConfiguration);

                return query
                    .Select(x => new ResponseSurveyUpload
                    {
                        ResponseSessionIdentifier = x.ResponseSession.ResponseSessionIdentifier,
                        SurveyFormIdentifier = x.ResponseSession.SurveyFormIdentifier,
                        SurveyQuestionIdentifier = x.SurveyQuestionIdentifier,
                        ResponseAnswerText = x.ResponseAnswerText,
                        SurveyFormName = x.SurveyQuestion.SurveyForm.SurveyFormName,
                        ResponseSessionStarted = x.ResponseSession.ResponseSessionStarted,
                        SurveyQuestionSequence = x.SurveyQuestion.SurveyQuestionSequence,
                        RespondentUserIdentifier = x.ResponseSession.RespondentUserIdentifier
                    })
                    .ToList();
            }
        }

        #region Surveys

        public bool Exists(Guid survey)
        {
            using (var db = CreateContext())
                return db.QSurveyForms.Any(x => x.SurveyFormIdentifier == survey);
        }

        public SurveyState GetSurveyState(Guid survey)
        {
            return _aggregateSearch.GetState<SurveyAggregate>(survey) as SurveyState;
        }

        public QSurveyForm GetSurveyForm(Guid surveyFormIdentifier, params Expression<Func<QSurveyForm, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QSurveyForms.Where(x => x.SurveyFormIdentifier == surveyFormIdentifier).ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        public QSurveyForm GetSurveyFormByAsset(Guid organization, int asset)
        {
            using (var db = CreateContext())
                return db.QSurveyForms.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.AssetNumber == asset);
        }

        public QSurveyForm GetSurveyFormByHook(Guid organization, string hook)
        {
            using (var db = CreateContext())
                return db.QSurveyForms.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.SurveyFormHook == hook);
        }

        public QSurveyForm GetSurveyFormByName(Guid organization, string name)
        {
            using (var db = CreateContext())
                return db.QSurveyForms.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.SurveyFormName == name);
        }

        public int CountSurveyForms(QSurveyFormFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QSurveyForm> GetSurveyForms(QSurveyFormFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                if (filter.SortByColumn.IsNotEmpty())
                    query = query.OrderBy(filter.SortByColumn);
                else
                    query = query.OrderBy(x => x.SurveyFormName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<QSurveyForm> CreateQuery(QSurveyFormFilter filter, InternalDbContext db)
        {
            var query = db.QSurveyForms
                .AsQueryable()
                .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.Name.IsNotEmpty())
                query = query.Where(x => x.SurveyFormName.Contains(filter.Name));

            if (filter.Title.IsNotEmpty())
                query = query.Where(x => db.TContents.Any(y => y.ContainerIdentifier == x.SurveyFormIdentifier && y.ContentLabel == "Title" && y.ContentText.Contains(filter.Title)));

            if (filter.Identifier.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier == filter.Identifier);

            if (filter.Identifiers.IsNotEmpty())
                query = query.Where(x => filter.Identifiers.Any(y => y == x.SurveyFormIdentifier));

            if (filter.CurrentStatus.IsNotEmpty())
                query = query.Where(x => filter.CurrentStatus.Contains(x.SurveyFormStatus));

            if (filter.EnableUserConfidentiality.HasValue)
                query = query.Where(x => x.EnableUserConfidentiality == filter.EnableUserConfidentiality);

            if (filter.IsLocked.HasValue)
            {
                if (filter.IsLocked.Value)
                    query = query.Where(x => x.SurveyFormLocked != null);
                else
                    query = query.Where(x => x.SurveyFormLocked == null);
            }

            if (filter.MessageIdentifier.HasValue)
                query = query.Where(x => x.SurveyMessageInvitation == filter.MessageIdentifier);

            if (filter.LastModifiedSince.HasValue)
                query = query.Where(x => x.LastChangeTime >= filter.LastModifiedSince);

            if (filter.LastModifiedBefore.HasValue)
                query = query.Where(x => x.LastChangeTime < filter.LastModifiedBefore);

            return query;
        }

        #endregion

        #region Questions

        public List<QSurveyQuestion> GetSurveyQuestions(QSurveyQuestionFilter filter, params Expression<Func<QSurveyQuestion, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = query.OrderBy(x => x.SurveyQuestionSequence).ApplyIncludes(includes);

                return query.ToList();
            }
        }

        private static IQueryable<QSurveyQuestion> CreateQuery(QSurveyQuestionFilter filter, InternalDbContext db)
        {
            var query = db.QSurveyQuestions
                .AsQueryable();

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            if (filter.HasResponseAnswer.HasValue)
            {
                if (filter.HasResponseAnswer.Value)
                    query = query.Where(x => x.QResponseAnswers.Any());
                else
                    query = query.Where(x => !x.QResponseAnswers.Any());
            }

            if (filter.ExcludeQuestionsID.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeQuestionsID.Contains(x.SurveyQuestionIdentifier));

            if (filter.ExcludeQuestionsTypes.IsNotEmpty())
            {
                var exlcludeFilter = filter.ExcludeQuestionsTypes.Select(x => x.ToString()).ToArray();
                query = query.Where(x => !exlcludeFilter.Contains(x.SurveyQuestionType));
            }

            if (filter.IncludeQuestionsTypes.IsNotEmpty())
            {
                var includeFilter = filter.IncludeQuestionsTypes.Select(x => x.ToString()).ToArray();
                query = query.Where(x => includeFilter.Contains(x.SurveyQuestionType));
            }

            if (filter.HasOptions.HasValue)
            {
                if (filter.HasOptions.Value)
                    query = query.Where(x => x.QSurveyOptionLists.Any(y => y.QSurveyOptionItems.Any()));
                else
                    query = query.Where(x => !x.QSurveyOptionLists.Any(y => y.QSurveyOptionItems.Any()));
            }

            return query;
        }

        public QSurveyQuestion GetSurveyQuestion(Guid questionId, params Expression<Func<QSurveyQuestion, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QSurveyQuestions.Where(x => x.SurveyQuestionIdentifier == questionId).ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        #endregion

        #region Option Lists

        public List<QSurveyOptionList> GetSurveyOptionLists(QSurveyOptionListFilter filter, params Expression<Func<QSurveyOptionList, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = query.OrderBy(x => x.SurveyOptionListSequence).ApplyIncludes(includes);

                return query.ToList();
            }
        }

        private static IQueryable<QSurveyOptionList> CreateQuery(QSurveyOptionListFilter filter, InternalDbContext db)
        {
            var query = db.QSurveyOptionLists
                .AsQueryable();

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyQuestion.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            if (filter.SurveyQuestionIdentifier.HasValue)
                query = query.Where(x => x.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier);

            return query;

        }

        public QSurveyOptionList GetSurveyOptionList(Guid optionListId, params Expression<Func<QSurveyOptionList, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QSurveyOptionLists.Where(x => x.SurveyOptionListIdentifier == optionListId).ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        #endregion

        #region Option Items

        public List<QSurveyOptionItem> GetSurveyOptionItems(QSurveyOptionItemFilter filter, params Expression<Func<QSurveyOptionItem, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                query = query.OrderBy(x => x.SurveyOptionItemSequence).ApplyIncludes(includes);

                return query.ToList();
            }
        }

        private static IQueryable<QSurveyOptionItem> CreateQuery(QSurveyOptionItemFilter filter, InternalDbContext db)
        {
            var query = db.QSurveyOptionItems
                .AsQueryable();

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyOptionList.SurveyQuestion.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            if (filter.SurveyQuestionIdentifier.HasValue)
                query = query.Where(x => x.SurveyOptionList.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier);

            if (filter.SurveyOptionListIdentifier.HasValue)
                query = query.Where(x => x.SurveyOptionListIdentifier == filter.SurveyOptionListIdentifier);

            return query;

        }

        public QSurveyOptionItem GetSurveyOptionItem(Guid optionId, params Expression<Func<QSurveyOptionItem, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QSurveyOptionItems.Where(x => x.SurveyOptionItemIdentifier == optionId).ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        #endregion

        #region Conditions

        public List<QSurveyCondition> GetSurveyConditions(QSurveyConditionFilter filter, params Expression<Func<QSurveyCondition, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                return query.ToList();
            }
        }

        private static IQueryable<QSurveyCondition> CreateQuery(QSurveyConditionFilter filter, InternalDbContext db)
        {
            var query = db.QSurveyConditions
                .AsQueryable();

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.MaskingSurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            return query;

        }

        public QSurveyCondition GetSurveyCondition(Guid optionItemId, Guid questionId, params Expression<Func<QSurveyCondition, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QSurveyConditions
                    .Where(x => x.MaskingSurveyOptionItemIdentifier == optionItemId && x.MaskedSurveyQuestionIdentifier == questionId)
                    .ApplyIncludes(includes);

                return query.FirstOrDefault();
            }
        }

        #endregion

        #region Responses

        public List<VUser> GetUsersWithMultiResponseSessions(Guid surveyForm)
        {
            using (var db = new InternalDbContext())
            {
                return db.QResponseSessions
                    .Where(x => x.SurveyFormIdentifier == surveyForm)
                    .GroupBy(x => x.Respondent)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .OrderBy(x => x.UserFullName)
                    .ToList();
            }
        }

        public static VUser GetUser(string email)
        {
            using (var db = new InternalDbContext())
            {
                return db.VUsers.FirstOrDefault(x => x.UserEmail == email);
            }
        }

        public QResponseSession[] GetOrphanResponses()
        {
            using (var db = new InternalDbContext())
            {
                return db.QResponseSessions
                    .Where(x => !db.Users.Any(y => y.UserIdentifier == x.RespondentUserIdentifier))
                    .ToArray();
            }
        }

        public int CountResponseAnswers(Guid surveyQuestionIdentifier)
        {
            using (var db = CreateContext())
                return db.QResponseAnswers.Where(x => x.SurveyQuestionIdentifier == surveyQuestionIdentifier).Count();
        }

        public string FirstCommentAnswer(Guid session)
        {
            using (var db = CreateContext())
            {
                var field = db.QResponseAnswers
                    .AsNoTracking()
                    .Where(x => x.ResponseSessionIdentifier == session)
                    .Where(x => x.SurveyQuestion.SurveyQuestionType == "Comment")
                    .OrderBy(x => x.SurveyQuestion.SurveyQuestionSequence)
                    .FirstOrDefault();
                return field?.ResponseAnswerText;
            }
        }

        public bool HasResponseOptions(Guid surveyOptionIdentifier)
        {
            using (var db = CreateContext())
                return db.QResponseOptions.Any(x => x.SurveyOptionIdentifier == surveyOptionIdentifier);
        }

        public QResponseOption GetResponseOption(Guid responseSessionIdentifier, Guid surveyOptionIdentifier)
        {
            using (var db = CreateContext())
                return db.QResponseOptions.FirstOrDefault(x => x.ResponseSessionIdentifier == responseSessionIdentifier && x.SurveyOptionIdentifier == surveyOptionIdentifier);
        }

        public bool HasResponseSessions(QResponseSessionFilter filter)
        {
            using (var db = CreateContext())
                return SearchQuery(filter, db).FirstOrDefault() != null;
        }

        public int CountResponseSessions(QResponseSessionFilter filter)
        {
            using (var db = CreateContext())
                return CountQuery(filter, db).Count();
        }

        public int CountResponseSessions(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public QResponseSession GetResponseSession(Guid session, params Expression<Func<QResponseSession, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QResponseSessions.ApplyIncludes(includes);

                return query.FirstOrDefault(x => x.ResponseSessionIdentifier == session);
            }
        }

        public ISurveyResponse GetResponseSession(QResponseSessionFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = SearchQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                {
                    query = query
                        .OrderBy(filter.OrderBy);
                }
                else
                {
                    query = query
                        .OrderByDescending(x => x.ResponseSessionStarted)
                        .ThenBy(x => x.RespondentName);
                }

                return query.FirstOrDefault();
            }
        }

        public List<ISurveyResponse> GetResponseSessions(Guid form, Guid user)
        {
            var filter = new QResponseSessionFilter();
            filter.SurveyFormIdentifier = form;
            filter.RespondentUserIdentifier = user;

            using (var db = CreateContext())
                return SearchQuery(filter, db).ToList();
        }

        public static List<string> SelectContactsWithMultiSubmissions(Guid form)
        {
            using (var db = new InternalDbContext())
            {
                return db.QResponseSessions
                    .Where(x => x.SurveyFormIdentifier == form)
                    .GroupBy(x => x.Respondent)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .OrderBy(x => x.UserFullName)
                    .Select(x => x.UserFullName)
                    .ToList();
            }
        }

        public List<ISurveyResponse> GetResponseSessions(Guid form, Guid[] users)
        {
            var filter = new QResponseSessionFilter();
            filter.SurveyFormIdentifier = form;
            filter.RespondentUserIdentifiers = users;

            using (var db = CreateContext())
                return SearchQuery(filter, db).ToList();
        }

        public List<ISurveyResponse> GetResponseSessions(Guid user)
        {
            var filter = new QResponseSessionFilter();
            filter.RespondentUserIdentifier = user;

            using (var db = CreateContext())
                return SearchQuery(filter, db).ToList();
        }

        public List<ISurveyResponse> GetResponseSessions(QResponseSessionFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = SearchQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                {
                    query = query
                        .OrderBy(filter.OrderBy);
                }
                else
                {
                    query = query
                        .OrderByDescending(x => x.ResponseSessionStarted)
                        .ThenBy(x => x.RespondentName);
                }

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<ISurveyResponse> SearchResponses(QResponseSessionFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = SearchQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                {
                    query = query
                        .OrderBy(filter.OrderBy);
                }
                else
                {
                    query = query
                        .OrderByDescending(x => x.ResponseSessionStarted)
                        .ThenBy(x => x.RespondentName);
                }

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<ResponseAnalysisSelectionItem> GetSelectionAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.RadioList.GetName(),
                    SurveyQuestionType.Selection.GetName()
                };

                return CreateQuery(filter, db)
                    .Join(db.QResponseOptions,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        x.ResponseOptionIsSelected == true
                        && typeFilter.Contains(x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionType))
                    .GroupBy(x => new
                    {
                        x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier,
                        x.SurveyOptionIdentifier,
                        x.SurveyOptionItem.SurveyOptionItemPoints
                    })
                    .Select(x => new ResponseAnalysisSelectionItem
                    {
                        QuestionIdentifier = x.Key.SurveyQuestionIdentifier,
                        OptionIdentifier = x.Key.SurveyOptionIdentifier,
                        OptionPoints = x.Key.SurveyOptionItemPoints,
                        AnswerCount = x.Count()
                    })
                    .ToList();
            }
        }

        public List<ResponseAnalysisChecklistItem> GetChecklistAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.CheckList.GetName(),
                    SurveyQuestionType.Likert.GetName()
                };

                return CreateQuery(filter, db)
                    .Join(db.QResponseOptions,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        x.ResponseOptionIsSelected == true
                        && typeFilter.Contains(x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionType))
                    .OrderBy(x => x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionSequence)
                    .ThenBy(x => x.SurveyOptionItem.SurveyOptionList.SurveyOptionListSequence)
                    .ThenBy(x => x.SurveyOptionItem.SurveyOptionItemSequence)
                    .Select(x => new ResponseAnalysisChecklistItem
                    {
                        QuestionIdentifier = x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier,
                        OptionIdentifier = x.SurveyOptionIdentifier,
                        OptionPoints = x.SurveyOptionItem.SurveyOptionItemPoints ?? 0
                    })
                    .ToList();
            }
        }

        public List<ResponseAnalysisCategoryItem> GetCategoryAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.RadioList.GetName(),
                };

                var temp = CreateQuery(filter, db)
                    .Join(db.QResponseOptions,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    ).ToList();

                return CreateQuery(filter, db)
                    .Join(db.QResponseOptions,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        x.ResponseOptionIsSelected == true
                        && typeFilter.Contains(x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionType))
                    .GroupBy(x => new
                    {
                        x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier,
                        x.SurveyOptionItem.SurveyOptionItemCategory
                    })
                    .Select(x => new ResponseAnalysisCategoryItem
                    {
                        QuestionIdentifier = x.Key.SurveyQuestionIdentifier,
                        OptionCategory = x.Key.SurveyOptionItemCategory,
                        AnswerCount = x.Count()
                    })
                    .ToList();
            }
        }

        public List<ResponseAnalysisIntegerItem> GetIntegerAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.Number.GetName()
                };

                return CreateQuery(filter, db)
                    .Join(db.QResponseAnswers,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        x.ResponseAnswerText != null
                        && typeFilter.Contains(x.SurveyQuestion.SurveyQuestionType))
                    .ToArray()
                    .Select(x => new
                    {
                        x.SurveyQuestionIdentifier,
                        AnswerNumber = double.TryParse(x.ResponseAnswerText, out var d) ? d : (double?)null
                    })
                    .Where(x => x.AnswerNumber.HasValue)
                    .GroupBy(x => x.SurveyQuestionIdentifier)
                    .Select(x =>
                    {
                        var count = x.Count();
                        return new ResponseAnalysisIntegerItem
                        {
                            QuestionIdentifier = x.Key,
                            Minimum = x.Min(y => y.AnswerNumber.Value),
                            Maximum = x.Max(y => y.AnswerNumber.Value),
                            Sum = x.Sum(y => y.AnswerNumber.Value),
                            Average = x.Average(y => y.AnswerNumber.Value),
                            StandardDeviation = count > 1 ? Calculator.CalculateStandardDeviation(x.Select(y => y.AnswerNumber.Value)) : (double?)null,
                            Variance = Calculator.CalculatePopulationVariance(x.Select(y => y.AnswerNumber.Value)),
                            Count = count
                        };
                    })
                    .ToList();
            }
        }

        public List<ResponseAnalysisTextItem> GetTextAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.RadioList.GetName(),
                    SurveyQuestionType.Selection.GetName(),
                    SurveyQuestionType.Likert.GetName(),
                    SurveyQuestionType.CheckList.GetName()
                };

                var query = CreateQuery(filter, db);

                var selectionsQuery = query
                    .Join(db.QResponseOptions,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        typeFilter.Contains(x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionType)
                        && x.ResponseOptionIsSelected == true
                    )
                    .Select(x => new
                    {
                        x.ResponseSessionIdentifier,
                        x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier,
                        OptionTitle = db.TContents
                            .Where(c =>
                                c.ContainerIdentifier == x.SurveyOptionIdentifier
                                && c.ContentLabel == ContentLabel.Title
                                && c.ContentLanguage == "en")
                            .Select(c => c.ContentText)
                            .FirstOrDefault()
                    })
                    .Where(x => x.OptionTitle != null);

                var nonSelectionsQuery = query
                    .Join(db.QResponseAnswers,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => b
                    )
                    .Where(x =>
                        !typeFilter.Contains(x.SurveyQuestion.SurveyQuestionType)
                        && x.ResponseAnswerText != null)
                    .Select(x => new
                    {
                        x.ResponseSessionIdentifier,
                        x.SurveyQuestionIdentifier,
                        AnswerText = x.ResponseAnswerText
                    });

                var list = new List<ResponseAnalysisTextItem>();

                foreach (var item in selectionsQuery)
                    list.Add(new ResponseAnalysisTextItem
                    {
                        ResponseSessionIdentifier = item.ResponseSessionIdentifier,
                        QuestionIdentifier = item.SurveyQuestionIdentifier,
                        AnswerText = item.OptionTitle
                    });

                foreach (var item in nonSelectionsQuery)
                    list.Add(new ResponseAnalysisTextItem
                    {
                        ResponseSessionIdentifier = item.ResponseSessionIdentifier,
                        QuestionIdentifier = item.SurveyQuestionIdentifier,
                        AnswerText = StringHelper.StripHtml(item.AnswerText?.Replace("\n", "").Replace("\r", ""))
                    });

                list.Sort((a, b) => string.Compare(a.AnswerText, b.AnswerText));

                return list;
            }
        }

        public List<ResponseAnalysisCommentItem> GetCommentAnalysis(QResponseAnalysisFilter filter)
        {
            using (var db = CreateContext())
            {
                var typeFilter = new[]
                {
                    SurveyQuestionType.CheckList.GetName(),
                    SurveyQuestionType.Comment.GetName(),
                    SurveyQuestionType.RadioList.GetName(),
                    SurveyQuestionType.Likert.GetName(),
                };

                var data = CreateQuery(filter, db)
                    .Join(db.QResponseAnswers,
                        a => a.ResponseSessionIdentifier,
                        b => b.ResponseSessionIdentifier,
                        (a, b) => new { Response = a, Answer = b }
                    )
                    .Where(x =>
                        x.Answer.ResponseAnswerText != null &&
                        typeFilter.Contains(x.Answer.SurveyQuestion.SurveyQuestionType))
                    .OrderBy(x => x.Answer.ResponseAnswerText)
                    .Select(x => new ResponseAnalysisCommentItem
                    {
                        ResponseSessionIdentifier = x.Response.ResponseSessionIdentifier,
                        QuestionIdentifier = x.Answer.SurveyQuestionIdentifier,
                        AnswerComment = x.Answer.ResponseAnswerText
                    })
                    .ToList();

                foreach (var item in data)
                    item.AnswerComment = StringHelper.StripHtml(item.AnswerComment);

                return data;
            }
        }

        public List<ResponseAnalysisCorrelationItem> GetCorrelationAnalysis(Guid xAxisQuestionId, Guid yAxisQuestionId)
        {
            const string query = "EXEC surveys.QResponseAnalysis @QuestionIdX, @QuestionIdY";

            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                return db.Database
                    .SqlQuery<ResponseAnalysisCorrelationItem>(
                        query,
                        new SqlParameter("@QuestionIdX", xAxisQuestionId),
                        new SqlParameter("@QuestionIdY", yAxisQuestionId)
                    )
                    .ToList();
            }
        }

        private static IQueryable<ISurveyResponse> CountQuery(QResponseSessionFilter filter, InternalDbContext db)
        {
            var query = db.QResponseSessions.AsNoTracking().AsQueryable();
            return ApplyFilter(query, filter, db);
        }

        private static IQueryable<ISurveyResponse> SearchQuery(QResponseSessionFilter filter, InternalDbContext db)
        {
            var query = db.VResponses.AsNoTracking().AsQueryable();
            return ApplyFilter(query, filter, db);
        }

        private static IQueryable<ISurveyResponse> ApplyFilter(
            IQueryable<ISurveyResponse> query, QResponseSessionFilter filter, InternalDbContext db
            )
        {
            if (filter.AgencyGroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.AgencyGroupIdentifier.Value);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => db.Memberships.Any(m => m.GroupIdentifier == filter.GroupIdentifier && m.UserIdentifier == x.RespondentUserIdentifier));

            if (filter.IsLocked.HasValue)
                query = query.Where(x => x.ResponseIsLocked == filter.IsLocked);

            if (filter.PeriodIdentifiers != null && filter.PeriodIdentifiers.Length > 0)
                query = query.Where(x => filter.PeriodIdentifiers.Any(y => y == x.PeriodIdentifier));

            if (filter.RespondentUserIdentifier.HasValue)
                query = query.Where(x => x.RespondentUserIdentifier == filter.RespondentUserIdentifier);

            if (filter.RespondentUserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.RespondentUserIdentifiers.Any(y => y == x.RespondentUserIdentifier));

            if (filter.RespondentName.IsNotEmpty())
            {
                if (!filter.IsPlatformAdministrator)
                {
                    // If this search is performed by someone who is not a Platform Administrator,
                    // then a query on respondent name is permitted only for surveys where
                    // respondent confidentiality is not enabled.

                    query = query.Join(db.QSurveyForms
                        .Where(survey => !survey.EnableUserConfidentiality),
                            response => response.SurveyFormIdentifier,
                            survey => survey.SurveyFormIdentifier,
                            (response, survey) => response
                        );
                }

                query = query.Join(db.QUsers.Where(user => user.FullName.StartsWith(filter.RespondentName)),
                    response => response.RespondentUserIdentifier,
                    user => user.UserIdentifier,
                    (response, person) => response
                );
            }

            if (filter.SurveyFormIdentifier.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            if (filter.SurveyQuestionIdentifier.HasValue)
                query = ApplyResponseFilterForQuestion(filter, db, query);

            if (filter.StartedSince.HasValue)
                query = query.Where(x => x.ResponseSessionStarted >= filter.StartedSince);

            if (filter.StartedBefore.HasValue)
                query = query.Where(x => x.ResponseSessionStarted < filter.StartedBefore);

            if (filter.CompletedSince.HasValue)
                query = query.Where(x => x.ResponseSessionCompleted >= filter.CompletedSince);

            if (filter.CompletedBefore.HasValue)
                query = query.Where(x => x.ResponseSessionCompleted < filter.CompletedBefore);

            if (filter.OrganizationIdentifier != Guid.Empty)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            return query;
        }

        private static IQueryable<ISurveyResponse> ApplyResponseFilterForQuestion(QResponseSessionFilter filter, InternalDbContext db, IQueryable<ISurveyResponse> query)
        {
            if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsEmpty())
            {
                var responsesContainingAnswerToQuestion = db.QResponseAnswers
                    .Where(y => y.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier)
                    .Select(y => y.ResponseSessionIdentifier)
                    .Distinct();

                query = query.Where(x => responsesContainingAnswerToQuestion.Any(y => y == x.ResponseSessionIdentifier));
            }
            else if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsNotEmpty())
            {
                var question = db.QSurveyQuestions
                    .Where(x => x.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier)
                    .FirstOrDefault();

                if (question == null)
                {
                    query = query.Where(x => 1 == 0);
                }
                else
                {
                    var type = question.SurveyQuestionType.ToEnum<SurveyQuestionType>();

                    if (type == SurveyQuestionType.CheckList || type == SurveyQuestionType.RadioList || type == SurveyQuestionType.Selection || type == SurveyQuestionType.Likert)
                    {
                        var responsesContainingOptionSelection = db.QResponseOptions
                            .Where(y => y.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && y.ResponseOptionIsSelected)
                            .Join(db.TContents.Where(y => y.ContentText == filter.ResponseAnswerText),
                                a => new { ContainerIdentifier = a.SurveyOptionIdentifier, ContentLabel = "Title", ContentLanguage = "en" },
                                b => new { b.ContainerIdentifier, b.ContentLabel, b.ContentLanguage },
                                (a, b) => a.ResponseSessionIdentifier
                            )
                            .Select(y => y)
                            .Distinct();

                        query = query.Where(x => responsesContainingOptionSelection.Any(y => y == x.ResponseSessionIdentifier));
                    }
                    else
                    {
                        var responsesContainingAnswerToQuestion = db.QResponseAnswers
                            .Where(y => y.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && y.ResponseAnswerText.Replace("\n", "").Replace("\r", "") == filter.ResponseAnswerText)
                            .Select(y => y.ResponseSessionIdentifier)
                            .Distinct();

                        query = query.Where(x => responsesContainingAnswerToQuestion.Any(y => y == x.ResponseSessionIdentifier));
                    }
                }
            }

            return query;
        }

        private static IQueryable<VResponse> ApplyResponseFilterForQuestion(QResponseSessionFilter filter, InternalDbContext db, IQueryable<VResponse> query)
        {
            if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsEmpty())
            {
                var list = db.QResponseAnswers
                    .Where(a => a.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier)
                    .Select(a => a.ResponseSessionIdentifier);

                query = query.Where(x => list.Any(item => item == x.ResponseSessionIdentifier));
            }
            else if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsNotEmpty())
            {
                var question = db.QSurveyQuestions
                    .Where(x => x.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier)
                    .FirstOrDefault();

                if (question == null)
                {
                    query = query.Where(x => 1 == 0);
                }
                else
                {
                    var type = question.SurveyQuestionType.ToEnum<SurveyQuestionType>();

                    if (type == SurveyQuestionType.CheckList || type == SurveyQuestionType.RadioList || type == SurveyQuestionType.Selection || type == SurveyQuestionType.Likert)
                    {
                        var list = db.QResponseOptions
                            .Where(a => a.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && a.ResponseOptionIsSelected)
                            .Join(db.TContents.Where(y => y.ContentText == filter.ResponseAnswerText),
                                a => new { ContainerIdentifier = a.SurveyOptionIdentifier, ContentLabel = "Title", ContentLanguage = "en" },
                                b => new { b.ContainerIdentifier, b.ContentLabel, b.ContentLanguage },
                                (a, b) => a.ResponseSessionIdentifier
                            )
                            .Select(a => a);

                        query = query.Where(response => list.Any(item => item == response.ResponseSessionIdentifier));
                    }
                    else
                    {
                        var list = db.QResponseAnswers
                            .Where(a => a.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && a.ResponseAnswerText.Replace("\n", "").Replace("\r", "") == filter.ResponseAnswerText)
                            .Select(a => a.ResponseSessionIdentifier);

                        query = query.Where(x => list.Any(item => item == x.ResponseSessionIdentifier));
                    }
                }
            }

            return query;
        }

        private static IQueryable<QResponseSession> CreateQuery(QResponseAnalysisFilter filter, InternalDbContext db)
        {
            var query = db.QResponseSessions.AsQueryable()
                .Where(x => x.SurveyFormIdentifier == filter.SurveyFormIdentifier);

            if (filter.IsLocked.HasValue)
                query = query.Where(x => x.ResponseIsLocked == filter.IsLocked);

            if (filter.IsPlatformAdministrator)
            {
                if (filter.RespondentName.IsNotEmpty())
                    query = query.Where(x => x.Respondent.UserFullName.Contains(filter.RespondentName));
            }
            else
            {
                if (filter.RespondentName.IsNotEmpty())
                    query = query.Where(x => !x.SurveyForm.EnableUserConfidentiality && x.Respondent.UserFullName.Contains(filter.RespondentName));
            }

            if (filter.StartedSince.HasValue)
                query = query.Where(x => x.ResponseSessionStarted >= filter.StartedSince);

            if (filter.StartedBefore.HasValue)
                query = query.Where(x => x.ResponseSessionStarted < filter.StartedBefore);

            if (filter.CompletedSince.HasValue)
                query = query.Where(x => x.ResponseSessionCompleted >= filter.CompletedSince);

            if (filter.CompletedBefore.HasValue)
                query = query.Where(x => x.ResponseSessionCompleted < filter.CompletedBefore);

            if (filter.EnableQuestionFilter && filter.AnswerFilter.Count > 0)
            {
                if (filter.AnswerFilterOperator == "or")
                {
                    var predicate = PredicateBuilder.False<QResponseSession>();
                    foreach (var filterItem in filter.AnswerFilter)
                    {
                        var questionFilter = GetQuestionFilterQuery(filterItem.QuestionIdentifier, filterItem.AnswerText);
                        predicate = filterItem.ComparisonType == ComparisonType.Equals
                            ? predicate.Or(x => questionFilter.Contains(x.ResponseSessionIdentifier))
                            : predicate.Or(x => !questionFilter.Contains(x.ResponseSessionIdentifier));
                    }
                    query = query.AsExpandable().Where(predicate);
                }
                else
                {
                    foreach (var filterItem in filter.AnswerFilter)
                    {
                        var questionFilter = GetQuestionFilterQuery(filterItem.QuestionIdentifier, filterItem.AnswerText);
                        query = filterItem.ComparisonType == ComparisonType.Equals
                            ? query.Where(x => questionFilter.Contains(x.ResponseSessionIdentifier))
                            : query.Where(x => !questionFilter.Contains(x.ResponseSessionIdentifier));
                    }
                }
            }

            if (filter.SurveyQuestionIdentifier.HasValue)
                query = ApplyResponseFilterForQuestion(filter, db, query);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => db.Memberships.Any(m => m.GroupIdentifier == filter.GroupIdentifier && m.UserIdentifier == x.RespondentUserIdentifier));

            return query;

            IQueryable<Guid> GetQuestionFilterQuery(Guid questionId, string answerText)
            {
                if (answerText.IsEmpty())
                {
                    return db.QResponseAnswers
                        .Where(x => x.SurveyQuestionIdentifier == questionId)
                        .Select(x => x.ResponseSessionIdentifier);
                }

                var question = db.QSurveyQuestions.FirstOrDefault(x => x.SurveyQuestionIdentifier == questionId);
                var questionType = question.SurveyQuestionType.ToEnum<SurveyQuestionType>();

                if (questionType == SurveyQuestionType.CheckList || questionType == SurveyQuestionType.RadioList || questionType == SurveyQuestionType.Selection || questionType == SurveyQuestionType.Likert)
                {
                    return db.QResponseSessions
                        .Where(r =>
                            r.QResponseOptions.Any(o =>
                                o.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier == questionId
                                && o.ResponseOptionIsSelected == true
                                && db.TContents.Any(c =>
                                    c.ContainerIdentifier == o.SurveyOptionIdentifier
                                    && c.ContentLabel == ContentLabel.Title
                                    && c.ContentLanguage == "en"
                                    && c.ContentText == answerText)
                            )
                        )
                        .Select(r => r.ResponseSessionIdentifier);
                }

                return db.QResponseSessions
                    .Where(r =>
                        r.QResponseAnswers.Any(a =>
                            a.SurveyQuestionIdentifier == questionId
                            && a.ResponseAnswerText.Replace("\n", "").Replace("\r", "") == answerText
                        )
                    )
                    .Select(r => r.ResponseSessionIdentifier);
            }
        }

        private static IQueryable<QResponseSession> ApplyResponseFilterForQuestion(QResponseAnalysisFilter filter, InternalDbContext db, IQueryable<QResponseSession> query)
        {
            if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsEmpty())
            {
                query = query.Where(x => x.QResponseAnswers.Any(y => y.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier));
            }
            else if (filter.SurveyQuestionIdentifier.HasValue && filter.ResponseAnswerText.IsNotEmpty())
            {
                var question = db.QSurveyQuestions
                    .Where(x => x.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier)
                    .FirstOrDefault();

                if (question == null)
                {
                    query = query.Where(x => 1 == 0);
                }
                else
                {
                    var type = question.SurveyQuestionType.ToEnum<SurveyQuestionType>();

                    if (type == SurveyQuestionType.CheckList || type == SurveyQuestionType.RadioList || type == SurveyQuestionType.Selection || type == SurveyQuestionType.Likert)
                    {
                        query = query.Where(x => x.QResponseOptions
                            .Where(y => y.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && y.ResponseOptionIsSelected)
                            .Join(db.TContents.Where(y => y.ContentText == filter.ResponseAnswerText),
                                a => new { ContainerIdentifier = a.SurveyOptionIdentifier, ContentLabel = "Title", ContentLanguage = "en" },
                                b => new { b.ContainerIdentifier, b.ContentLabel, b.ContentLanguage },
                                (a, b) => b.ContentText
                            )
                            .Any()
                        );
                    }
                    else
                    {
                        query = query.Where(x => x.QResponseAnswers
                            .Where(y => y.SurveyQuestionIdentifier == filter.SurveyQuestionIdentifier && y.ResponseAnswerText.Replace("\n", "").Replace("\r", "") == filter.ResponseAnswerText)
                            .Any()
                        );
                    }
                }
            }

            return query;
        }

        public string[] GetResponseAnswersText(Guid question)
        {
            using (var db = CreateContext())
            {
                var questionType = db.QSurveyQuestions
                    .Where(x => x.SurveyQuestionIdentifier == question)
                    .Select(x => x.SurveyQuestionType)
                    .FirstOrDefault();

                if (questionType.IsEmpty())
                    return new string[0];

                var type = questionType.ToEnum<SurveyQuestionType>();

                if (type == SurveyQuestionType.CheckList || type == SurveyQuestionType.RadioList || type == SurveyQuestionType.Selection || type == SurveyQuestionType.Likert)
                {
                    return db.QResponseOptions
                        .Where(x => x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier == question && x.ResponseOptionIsSelected)
                        .Join(db.TContents,
                            a => new { ContainerIdentifier = a.SurveyOptionIdentifier, ContentLabel = "Title", ContentLanguage = "en" },
                            b => new { b.ContainerIdentifier, b.ContentLabel, b.ContentLanguage },
                            (a, b) => b.ContentText
                        )
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray();
                }

                return db.QResponseAnswers
                    .Where(x => x.SurveyQuestionIdentifier == question && x.ResponseAnswerText != null)
                    .Select(x => x.ResponseAnswerText)
                    .Distinct()
                    .AsEnumerable()
                    .Select(x => x.Replace("\n", "").Replace("\r", ""))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
            }
        }

        public VSurveyResponseAnswer[] GetResponseAnswers(Guid question)
        {
            using (var db = CreateContext())
            {
                return db.VSurveyResponseAnswers
                    .Where(x => x.SurveyQuestionIdentifier == question)
                    .OrderByDescending(x => x.ResponseSessionStarted)
                    .ToArray();
            }
        }

        public VSurveyResponseAnswer[] GetResponseAnswers(Guid[] questions)
        {
            using (var db = CreateContext())
            {
                return db.VSurveyResponseAnswers
                    .Where(x => questions.Any(q => q == x.SurveyQuestionIdentifier))
                    .OrderByDescending(x => x.ResponseSessionStarted)
                    .ToArray();
            }
        }

        public QResponseAnswer[] GetAnswersByResponse(Guid surveyFormIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QResponseAnswers
                    .Where(x => x.ResponseSession.SurveyFormIdentifier == surveyFormIdentifier)
                    .OrderByDescending(x => x.ResponseSession.ResponseSessionStarted)
                    .ToArray();
            }
        }

        public QResponseOption[] GetOptionsByResponse(Guid surveyFormIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QResponseOptions
                    .Where(x => x.ResponseSession.SurveyFormIdentifier == surveyFormIdentifier)
                    .OrderByDescending(x => x.ResponseSession.ResponseSessionStarted)
                    .ToArray();
            }
        }

        #endregion

        #region Other

        public bool IsDuplicate(SurveyForm survey) =>
            IsDuplicate(survey.Tenant, survey.Identifier, survey.Name);

        public bool IsDuplicate(Guid organization, string name) =>
            IsDuplicate(organization, null, name);

        private bool IsDuplicate(Guid organization, Guid? surveyFormId, string name)
        {
            using (var db = new InternalDbContext())
                return IsDuplicate(organization, surveyFormId, name, db);
        }

        private bool IsDuplicate(Guid organization, Guid? surveyFormId, string name, InternalDbContext db)
        {
            var query = db.QSurveyForms
                .AsQueryable()
                .Where(x => x.OrganizationIdentifier == organization && x.SurveyFormName == name);

            if (surveyFormId.HasValue)
                query = query.Where(x => x.SurveyFormIdentifier != surveyFormId);

            return query.Any();
        }

        public bool IsValid(Guid surveyId)
        {
            using (var db = new InternalDbContext())
            {
                var conditions = db.QSurveyConditions.Where(x => x.MaskingSurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyFormIdentifier == surveyId).ToList();

                foreach (var condition in conditions)
                {
                    if (condition.MaskingSurveyOptionItem == null || condition.MaskedSurveyQuestion == null || condition.MaskingSurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionSequence > condition.MaskedSurveyQuestion.SurveyQuestionSequence)
                        return false;
                }
            }

            return true;
        }

        public VSurveyResponseSummary GetSurveyResponseSummary(Guid surveyFormId)
        {
            using (var db = CreateContext())
            {
                return db.SurveyResponseSummaries.FirstOrDefault(x => x.SurveyFormIdentifier == surveyFormId);
            }
        }

        public string GetValueFromColumn(string id, string schema, string table, string column, string idColumn)
        {
            const string query = "EXEC surveys.GetValueFromColumn @id, @schema, @table, @column, @idcolumn";

            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                return db.Database
                    .SqlQuery<string>(
                        query,
                        new SqlParameter("@id", id),
                        new SqlParameter("@schema", schema),
                        new SqlParameter("@table", table),
                        new SqlParameter("@column", column),
                        new SqlParameter("@idcolumn", idColumn)
                    ).FirstOrDefault()?.ToString();
            }
        }

        #endregion
    }
}
