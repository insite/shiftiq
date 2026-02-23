using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using InSite.Application.Attempts.Read;
using InSite.Application.Contents.Read;
using InSite.Domain.Attempts;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class AttemptSearch : IAttemptSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Classes

        private class AttemptReadHelper : ReadHelper<QAttempt>
        {
            public static readonly AttemptReadHelper Instance = new AttemptReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QAttempt>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.QAttempts.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Comments

        public QComment GetQAttemptComment(Guid attemptId, Guid questionIdentifier, Guid authorIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QComments
                    .AsNoTracking()
                    .Where(x => x.AssessmentAttemptIdentifier == attemptId && x.AssessmentQuestionIdentifier == questionIdentifier && x.AuthorUserIdentifier == authorIdentifier)
                    .FirstOrDefault();
            }
        }

        public List<QComment> GetQAttemptComments(Guid attemptId, Guid authorIdentifier)
        {
            using (var db = CreateContext())
            {
                return db.QComments
                    .AsNoTracking()
                    .Where(x => x.AssessmentAttemptIdentifier == attemptId && x.AuthorUserIdentifier == authorIdentifier)
                    .ToList();
            }
        }

        public List<VComment> GetVAttemptComments(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.VComments
                    .AsNoTracking()
                    .Where(x => x.AssessmentAttemptIdentifier == attempt)
                    .AsEnumerable()
                    .OrderByDescending(x => x.CommentPosted)
                    .ToList();
            }
        }

        public List<QAttemptCommentExtended> GetVAttemptComments(QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var attempts = CreateQuery(filter, db);

                var query = db.VComments.Where(comment => attempts.Any(attempt => attempt.AttemptIdentifier == comment.AssessmentAttemptIdentifier));

                if (filter.QuestionIdentifier.HasValue)
                    query = query.Where(x => x.AssessmentQuestionIdentifier == filter.QuestionIdentifier.Value);

                return query
                    .Select(x => new QAttemptCommentExtended
                    {
                        AttemptIdentifier = x.AssessmentAttemptIdentifier ?? Guid.Empty,
                        FormIdentifier = x.AssessmentFormIdentifier ?? Guid.Empty,
                        QuestionIdentifier = x.AssessmentQuestionIdentifier ?? Guid.Empty,
                        AuthorIdentifier = x.AuthorUserIdentifier,
                        AuthorName = x.AuthorUserName,
                        CommentPosted = x.CommentPosted,
                        CommentText = x.CommentText
                    })
                    .AsEnumerable()
                    .OrderByDescending(x => x.CommentPosted)
                    .ToList();
            }
        }

        public int CountExaminationFeedback(QAttemptCommentaryFilter filter)
        {
            using (var db = CreateContext())
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@OrganizationIdentifier", filter.OrganizationIdentifier)
                };

                var query = @"
select count(*)
from assessments.QAttempt                    as attempt
     inner join banks.QBankForm              as form on attempt.FormIdentifier = form.FormIdentifier
     inner join assessments.QAttemptQuestion as question on attempt.AttemptIdentifier = question.AttemptIdentifier
     inner join assets.QComment              as comment on (question.QuestionIdentifier = comment.AssessmentQuestionIdentifier)
                                                           and (question.AttemptIdentifier = comment.AssessmentAttemptIdentifier)
where form.OrganizationIdentifier = @OrganizationIdentifier";

                if (!string.IsNullOrEmpty(filter.FormTitle))
                {
                    parameters.Add(new SqlParameter("@FormTitle", $"%{filter.FormTitle}%"));
                    query += " and form.FormTitle like @FormTitle";
                }

                if (filter.AssetNumber.HasValue)
                {
                    parameters.Add(new SqlParameter("@FormAssetNumber", filter.AssetNumber));
                    query += " and form.FormAssetNumber = @FormAssetNumber";
                }

                return db.Database.SqlQuery<int>(query, parameters.ToArray()).FirstOrDefault();
            }
        }

        public List<QuestionCommentaryItem> SelectQuestionFeedbackForAnalysis(Guid questionId)
        {
            using (var db = CreateContext())
            {
                var sql = "exec assessments.SelectQuestionFeedbackForAnalysis @QuestionIdentifier";
                var param = new SqlParameter("QuestionIdentifier", questionId);

                var results = db.Database.SqlQuery<QuestionCommentaryItem>(sql, param);
                return results.ToList();
            }
        }

        public List<QAttemptCommentaryItem> SelectExaminationFeedback(QAttemptCommentaryFilter filter)
        {
            using (var db = CreateContext())
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@OrganizationIdentifier", filter.OrganizationIdentifier)
                };

                var query = @"
select form.FormTitle
     , form.FormAsset         as FormAssetNumber
     , question.QuestionSequence
     , question.QuestionText
     , comment.CommentPosted
     , comment.AuthorUserName as AuthorName
     , comment.CommentText
from assessments.QAttempt                    as attempt
     inner join banks.QBankForm              as form on attempt.FormIdentifier = form.FormIdentifier
     inner join assessments.QAttemptQuestion as question on attempt.AttemptIdentifier = question.AttemptIdentifier
     inner join assets.QComment              as comment on (question.QuestionIdentifier = comment.AssessmentQuestionIdentifier)
                                                           and (question.AttemptIdentifier = comment.AssessmentAttemptIdentifier)
where form.OrganizationIdentifier = @OrganizationIdentifier";

                if (!string.IsNullOrEmpty(filter.FormTitle))
                {
                    parameters.Add(new SqlParameter("@FormTitle", $"%{filter.FormTitle}%"));
                    query += " and form.FormTitle like @FormTitle";
                }

                if (filter.AssetNumber.HasValue)
                {
                    parameters.Add(new SqlParameter("@FormAssetNumber", filter.AssetNumber));
                    query += " and form.FormAsset = @FormAssetNumber";
                }

                if (filter.CommentText.HasValue())
                {
                    parameters.Add(new SqlParameter("@CommentText", $"%{filter.CommentText}%"));
                    query += " and comment.CommentText like @CommentText";
                }

                var results = db.Database.SqlQuery<QAttemptCommentaryItem>(query, parameters.ToArray());

                return results
                    .OrderBy(x => x.FormTitle)
                    .ThenBy(x => x.QuestionSequence)
                    .ThenBy(x => x.QuestionText)
                    .ThenBy(x => x.CommentPosted)
                    .ThenBy(x => x.AuthorName)
                    .ToList();
            }
        }

        #endregion

        #region Attempts

        public int CountAttempts(QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.Count();
            }
        }

        public int CountAttempts(Expression<Func<QAttempt, bool>> filter) =>
            AttemptReadHelper.Instance.Count(filter);

        public QAttempt GetAttempt(Guid attemptId, params Expression<Func<QAttempt, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QAttempts.AsNoTracking().ApplyIncludes(includes)
                    .FirstOrDefault(x => x.AttemptIdentifier == attemptId);
            }
        }

        public List<QAttempt> GetAttempts(Guid form, Guid user, params Expression<Func<QAttempt, object>>[] includes)
        {
            return GetAttempts(new QAttemptFilter
            {
                FormIdentifier = form,
                LearnerUserIdentifier = user
            }, includes);
        }

        public List<QAttempt> GetAttempts(QAttemptFilter filter, params Expression<Func<QAttempt, object>>[] includes)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                using (var db = CreateContext())
                {
                    var query = CreateQuery(filter, db).ApplyIncludes(includes);

                    query = string.IsNullOrEmpty(filter.OrderBy)
                        ? query.OrderByDescending(x => x.AttemptStarted)
                        : query.OrderBy(filter.OrderBy);

                    return query.ApplyPaging(filter).ToList();
                }
            }
        }

        public T[] BindAttempts<T>(Expression<Func<QAttempt, T>> binder, QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                return query.Select(binder).ToArray();
            }
        }

        public List<QAttempt> GetAttemptsByEvent(Guid @event, string filterText = null, Paging paging = null, bool includeQuestions = false, bool includeEvent = false)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttempts
                    .Include(x => x.LearnerPerson)
                    .Include(x => x.Form)
                    .AsNoTracking()
                    .AsQueryable();

                if (includeQuestions)
                    query = query.Include(x => x.Questions);

                if (includeEvent)
                    query = query.Include(x => x.Registration);

                query = query.Where(x => x.Registration.EventIdentifier == @event);

                if (!string.IsNullOrEmpty(filterText))
                {
                    query = query.Where(x => x.LearnerPerson.UserFullName.Contains(filterText)
                                          || x.LearnerPerson.UserEmail.Contains(filterText)
                                          || x.LearnerPerson.PersonCode.Contains(filterText));
                }

                return query
                    .OrderBy(x => x.LearnerPerson.UserFullName)
                    .ApplyPaging(paging)
                    .ToList();
            }
        }

        internal static IQueryable<QAttempt> CreateQuery(QAttemptFilter filter, InternalDbContext db)
        {
            var query = db.QAttempts.AsNoTracking().AsQueryable();

            if (filter.AttemptIdentifier.HasValue)
                query = query.Where(x => x.AttemptIdentifier == filter.AttemptIdentifier.Value);

            if (filter.AttemptIdentifiers != null && filter.AttemptIdentifiers.Length > 0)
                query = query.Where(x => filter.AttemptIdentifiers.Contains(x.AttemptIdentifier));

            if (filter.CandidateOrganizationIdentifiers.IsNotEmpty())
            {
                if (filter.OrganizationPersonTypes.IsNotEmpty())
                {
                    var isAdministrator = filter.OrganizationPersonTypes.Contains("Administrator");
                    var isLearner = filter.OrganizationPersonTypes.Contains("Learner");

                    query = query.Where(x => db.Persons.Any(y =>
                        y.UserIdentifier == x.LearnerUserIdentifier
                        && filter.CandidateOrganizationIdentifiers.Contains(y.OrganizationIdentifier)
                        && (
                            (isAdministrator && y.IsAdministrator)
                         || (isLearner && y.IsLearner)
                            )));
                }
                else
                {
                    query = query.Where(x => db.Persons.Any(y =>
                                                                y.UserIdentifier == x.LearnerUserIdentifier
                                                                && filter.CandidateOrganizationIdentifiers.Contains(y.OrganizationIdentifier)
                                                            ));
                }
            }

            if (filter.FormOrganizationIdentifier.HasValue)
                query = query.Where(x => x.Form.OrganizationIdentifier == filter.FormOrganizationIdentifier.Value);

            if (filter.LearnerOrganizationIdentifier.HasValue)
                query = query.Where(attempt => db.QPersons.Any(
                    person => person.UserIdentifier == attempt.LearnerUserIdentifier
                           && person.OrganizationIdentifier == filter.LearnerOrganizationIdentifier.Value));

            if (filter.GradingAssessorIdentifier.HasValue)
                query = query.Where(x => x.GradingAssessorUserIdentifier == filter.GradingAssessorIdentifier);

            if (filter.IsWithoutGradingAssessor.HasValue && filter.IsWithoutGradingAssessor.Value)
                query = query.Where(x => x.GradingAssessorUserIdentifier == null);

            if (filter.EventIdentifier.HasValue)
                query = query.Where(x => x.Registration.EventIdentifier == filter.EventIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.EventFormat))
                query = query.Where(x => x.Registration.Event.EventFormat == filter.EventFormat);

            if (filter.BankLevel.IsNotEmpty())
                query = query.Where(x => x.Form.Bank.BankLevel == filter.BankLevel);

            if (filter.BankIdentifier.HasValue && filter.BankIdentifier != Guid.Empty)
                query = query.Where(x => x.Form.BankIdentifier == filter.BankIdentifier.Value);

            if (filter.FormIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.FormIdentifiers.Contains(x.FormIdentifier));

            if (filter.QuestionIdentifier.HasValue)
                query = query.Where(x => x.Questions.Any(y => y.QuestionIdentifier == filter.QuestionIdentifier.Value));

            if (filter.OccupationIdentifier.HasValue)
                query = query.Where(x => x.Form.VBank.OccupationIdentifier == filter.OccupationIdentifier.Value);

            if (filter.FrameworkIdentifier.HasValue)
                query = query.Where(x => x.Form.Bank.FrameworkIdentifier == filter.FrameworkIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.LearnerKeyword))
            {
                var candidates = StringHelper.Split(filter.LearnerKeyword);
                query = query.Where(x => x.LearnerPerson.UserFullName.Contains(filter.LearnerKeyword)
                                 || x.LearnerPerson.UserEmail.Contains(filter.LearnerKeyword)
                                 || candidates.Contains(x.LearnerPerson.PersonCode)
                                 );
            }

            if (filter.LearnerUserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.LearnerUserIdentifiers.Contains(x.LearnerUserIdentifier));

            if (!string.IsNullOrEmpty(filter.AssessorName))
                query = query.Where(x => x.AssessorPerson.UserFullName.Contains(filter.AssessorName));

            if (!string.IsNullOrEmpty(filter.LearnerName))
                query = query.Where(x => x.LearnerPerson.UserFullName.Contains(filter.LearnerName));

            if (!string.IsNullOrEmpty(filter.LearnerEmail))
                query = query.Where(x => x.LearnerPerson.UserEmail.Contains(filter.LearnerEmail));

            if (!string.IsNullOrEmpty(filter.LearnerCompany))
                query = query.Where(x => x.LearnerPerson.EmployerGroupName.Contains(filter.LearnerCompany));

            if (!string.IsNullOrEmpty(filter.Form))
            {
                query = query.Where(x => x.Form.FormTitle.Contains(filter.Form)
                             || x.Form.FormName.Contains(filter.Form)
                             || x.Form.FormAsset.ToString() == filter.Form
                             );
            }

            if (!string.IsNullOrEmpty(filter.FormName))
                query = query.Where(x => x.Form.FormName.Contains(filter.FormName));

            if (!string.IsNullOrEmpty(filter.Event))
                query = query.Where(x => filter.Event == x.Registration.Event.EventNumber.ToString());

            if (filter.IsStarted.HasValue)
            {
                if (filter.IsStarted.Value)
                    query = query.Where(x => x.AttemptStarted.HasValue);
                else
                    query = query.Where(x => !x.AttemptStarted.HasValue);
            }

            if (filter.IsSubmitted.HasValue)
            {
                if (filter.IsSubmitted.Value)
                    query = query.Where(x => x.AttemptSubmitted.HasValue);
                else
                    query = query.Where(x => !x.AttemptSubmitted.HasValue);
            }

            if (filter.IsCompleted.HasValue)
            {
                if (filter.IsCompleted.Value)
                    query = query.Where(x => x.AttemptGraded.HasValue);
                else
                    query = query.Where(x => !x.AttemptGraded.HasValue);
            }

            if (filter.IsImported.HasValue)
            {
                if (filter.IsImported.Value)
                    query = query.Where(x => x.AttemptImported.HasValue);
                else
                    query = query.Where(x => !x.AttemptImported.HasValue);
            }

            if (filter.AttemptGrade.IsNotEmpty())
                query = query.Where(x => x.AttemptGrade == filter.AttemptGrade);

            if (filter.AttemptStatus.IsNotEmpty())
                query = query.Where(x => filter.AttemptStatus == filter.AttemptStatus);

            if (filter.AttemptTag.IsNotEmpty())
                query = query.Where(x => filter.AttemptTag.Contains(x.AttemptTag));

            if (filter.AttemptTagStatus.IsNotEmpty())
            {
                if (filter.AttemptTagStatus == "Tagged")
                    query = query.Where(x => x.AttemptTag != null);
                else if (filter.AttemptTagStatus == "Not Tagged")
                    query = query.Where(x => x.AttemptTag == null);
            }

            if (filter.AttemptScoreFrom.HasValue)
                query = query.Where(x => Math.Round(x.AttemptScore.Value * 100) >= filter.AttemptScoreFrom.Value);

            if (filter.AttemptScoreThru.HasValue)
                query = query.Where(x => Math.Round(x.AttemptScore.Value * 100) <= filter.AttemptScoreThru.Value);

            if (filter.AttemptStartedSince.HasValue)
                query = query.Where(x => x.AttemptStarted >= filter.AttemptStartedSince.Value);

            if (filter.AttemptStartedBefore.HasValue)
                query = query.Where(x => x.AttemptStarted < filter.AttemptStartedBefore.Value);

            if (filter.AttemptGradedSince.HasValue)
                query = query.Where(x => x.AttemptGraded >= filter.AttemptGradedSince.Value);

            if (filter.AttemptGradedBefore.HasValue)
                query = query.Where(x => x.AttemptGraded < filter.AttemptGradedBefore.Value);

            if (filter.ExcludeArchivedUsers)
                query = query.Where(x => x.LearnerPerson.UtcArchived == null);

            if (filter.PilotAttemptsInclusion == InclusionType.Exclude)
                query = query.Where(x => x.Form.FormFirstPublished.HasValue && x.AttemptGraded >= x.Form.FormFirstPublished.Value);
            else if (filter.PilotAttemptsInclusion == InclusionType.Only)
                query = query.Where(x => x.Form.FormFirstPublished.HasValue && x.AttemptGraded < x.Form.FormFirstPublished.Value);

            if (filter.RubricIdentifier != null)
            {
                query = query.Where(x =>
                    x.Questions.Join(db.BankQuestions.Where(y => y.RubricIdentifier == filter.RubricIdentifier),
                        question => question.QuestionIdentifier,
                        rubric => rubric.QuestionIdentifier,
                        (question, rubric) => question).Any());
            }

            if (filter.CandidateType.IsNotEmpty())
            {
                if (filter.CandidateType.Contains("Unknown"))
                    query = query.Where(x => filter.CandidateType.Contains(x.Registration.CandidateType) || x.Registration.CandidateType == null);
                else
                    query = query.Where(x => filter.CandidateType.Contains(x.Registration.CandidateType));
            }

            return query;
        }

        public static T[] BindAttempts<T>(
            Expression<Func<QAttempt, T>> binder,
            Expression<Func<QAttempt, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            AttemptReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public List<string> GetAttemptTags(Guid organizationId)
        {
            using (var db = CreateContext())
                return db.QAttempts
                    .Where(x => x.Form.OrganizationIdentifier == organizationId && x.AttemptTag != null)
                    .Select(x => x.AttemptTag)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        public Guid[] GetOrphanAttempts()
        {
            const string sql = @"
select AttemptIdentifier
from (
         select AttemptIdentifier
              , FormIdentifier
              , LearnerUserIdentifier
         from assessments.QAttempt
     ) as D
where not exists (
                     select *
                     from banks.QBankForm
                     where QBankForm.FormIdentifier = D.FormIdentifier
                 )
      or not exists (
                        select *
                        from identities.[User]
                        where [User].UserIdentifier = D.LearnerUserIdentifier
                    )
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Guid>(sql).ToArray();
        }

        #endregion

        #region Sections

        public QAttemptSection GetAttemptSection(Guid attempt, int section)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptSections
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.SectionIndex == section)
                    .FirstOrDefault();
            }
        }

        public List<QAttemptSection> GetAttemptSections(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptSections
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt)
                    .AsEnumerable()
                    .OrderBy(x => x.SectionIndex)
                    .ToList();
            }
        }

        #endregion

        #region Questions

        public Guid[] GetExistsQuestionIdentifiers(IEnumerable<Guid> questionIds)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptQuestions.AsNoTracking().AsQueryable()
                    .Where(x => questionIds.Contains(x.QuestionIdentifier)).Select(x => x.QuestionIdentifier)
                    .Distinct().ToArray();
            }
        }

        public List<QAttemptQuestion> GetAttemptQuestions(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptQuestions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt)
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ToList();
            }
        }

        public List<string> GetAttemptQuestionTypes(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptQuestions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt)
                    .Select(x => x.QuestionType)
                    .Distinct()
                    .ToList();
            }
        }

        public List<QAttemptQuestion> GetAttemptQuestionsBySequence(Guid attempt, int[] sequence)
        {
            if (sequence.IsEmpty())
                return new List<QAttemptQuestion>();

            using (var db = CreateContext())
            {
                return db.QAttemptQuestions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && sequence.Contains(x.QuestionSequence))
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ToList();
            }
        }

        public QAttemptQuestion GetAttemptQuestion(Guid attempt, int sequence)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptQuestions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.QuestionSequence == sequence)
                    .SingleOrDefault();
            }
        }

        public List<QAttemptQuestion> GetAttemptQuestions(Guid attempt, int? sectionIndex)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptQuestions
                    .AsNoTracking().AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (sectionIndex.HasValue)
                    query = query.Where(x => x.SectionIndex == sectionIndex.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ToList();
            }
        }

        public List<QAttemptQuestion> GetAttemptQuestions(QAttemptQuestionFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateAttemptQuestionQuery(filter, db)
                    .Include(x => x.Attempt.LearnerPerson)
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ToList();
            }
        }

        public List<AnswerState> GetAttemptQuestionsByLearner(Guid learner, Guid[] forms)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptQuestions
                    .AsNoTracking()
                    .Where(x => x.Attempt.LearnerUserIdentifier == learner);

                if (forms.Length > 0)
                    query = query.Where(x => forms.Any(f => f == x.Attempt.FormIdentifier));

                return query
                    .Select(x => new AnswerState
                    {
                        Answered = x.Attempt.AttemptGraded,
                        Question = x.QuestionIdentifier,
                        QuestionPoints = x.QuestionPoints,
                        AnswerPoints = x.AnswerPoints,
                        AttemptScore = x.Attempt.AttemptScore,
                        Competency = x.CompetencyItemIdentifier,
                        Form = x.Attempt.FormIdentifier,
                        AttemptIsPass = x.Attempt.AttemptIsPassing
                    })
                    .AsEnumerable()
                    .OrderByDescending(x => x.Answered)
                    .ToList();
            }
        }

        public T[] BindAttemptQuestions<T>(Expression<Func<QAttemptQuestion, T>> binder, QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).SelectMany(x => x.Questions.Select(y => y));

                query = query
                    .OrderByDescending(x => x.Attempt.AttemptStarted)
                    .ThenBy(x => x.QuestionSequence);

                return query.Select(binder).ToArray();
            }
        }

        public int CountAttemptQuestions(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptQuestions
                    .Where(x => x.AttemptIdentifier == attempt)
                    .Count();
            }
        }

        private IQueryable<QAttemptQuestion> CreateAttemptQuestionQuery(QAttemptQuestionFilter filter, InternalDbContext db)
        {
            var query = db.QAttemptQuestions.AsNoTracking().AsQueryable();

            if (filter.FormIdentifier.HasValue)
                query = query.Where(x => x.Attempt.FormIdentifier == filter.FormIdentifier.Value);

            if (filter.QuestionIdentifier.HasValue)
                query = query.Where(x => x.QuestionIdentifier == filter.QuestionIdentifier.Value);

            if (filter.LearnerUserIdentifier.HasValue)
                query = query.Where(x => x.Attempt.LearnerUserIdentifier == filter.LearnerUserIdentifier.Value);

            return query;
        }

        #endregion

        #region Options

        public List<int> GetAttemptExistOptionKeys(Guid question)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptOptions
                    .Where(x => x.QuestionIdentifier == question)
                    .Select(x => x.OptionKey)
                    .Distinct()
                    .ToList();
            }
        }

        public List<QAttemptOption> GetAttemptOptions(Guid attempt, Guid? question = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptOptions
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (question != null)
                    query = query.Where(x => x.QuestionIdentifier == question.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.OptionSequence)
                    .ToList();
            }
        }

        public List<QAttemptOption> GetAttemptOptions(Guid attempt, Guid[] questions)
        {
            if (questions.IsEmpty())
                return new List<QAttemptOption>();

            using (var db = CreateContext())
            {
                return db.QAttemptOptions
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt && questions.Contains(x.QuestionIdentifier))
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.OptionSequence)
                    .ToList();
            }
        }

        public List<QAttemptOption> GetAttemptOptions(QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .SelectMany(x => x.Options.Select(y => y));

                if (filter.QuestionIdentifier.HasValue)
                    query = query.Where(x => x.QuestionIdentifier == filter.QuestionIdentifier.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.OptionSequence)
                    .ToList();
            }
        }

        public T[] BindAttemptOptions<T>(Expression<Func<QAttemptOption, T>> binder, Guid attempt)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptOptions.AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt);

                return query.Select(binder).ToArray();
            }
        }

        public T[] BindAttemptOptions<T>(Expression<Func<QAttemptOption, T>> binder, QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db)
                    .SelectMany(x => x.Options.Select(y => y));

                if (filter.QuestionIdentifier.HasValue)
                    query = query.Where(x => x.QuestionIdentifier == filter.QuestionIdentifier.Value);

                return query
                    .Select(binder)
                    .ToArray();
            }
        }

        public int? GetAttemptOptionKeyBySequence(Guid attempt, Guid question, int optionSequence)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptOptions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.OptionSequence == optionSequence)
                    .Select(x => x.OptionKey)
                    .FirstOrDefault();
            }
        }

        public List<int> GetAttemptOptionKeysBySequence(Guid attempt, Guid question, int[] optionSequence)
        {
            if (optionSequence.IsEmpty())
                return new List<int>();

            using (var db = CreateContext())
            {
                return db.QAttemptOptions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && optionSequence.Contains(x.OptionSequence))
                    .Select(x => x.OptionKey)
                    .ToList();
            }
        }

        #endregion

        #region Matches

        public List<QAttemptMatch> GetAttemptMatches(Guid attempt, Guid? question = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptMatches
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (question != null)
                    query = query.Where(x => x.QuestionIdentifier == question.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.MatchSequence)
                    .ToList();
            }
        }

        public T[] BindAttemptMatches<T>(Expression<Func<QAttemptMatch, T>> binder, QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).SelectMany(x => x.Matches.Select(y => y));

                query = query
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.MatchSequence);

                return query.Select(binder).ToArray();
            }
        }

        #endregion

        #region Pins

        public List<QAttemptPin> GetAttemptPins(Guid attempt, Guid? question = null, int? option = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptPins.AsNoTracking().AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (question.HasValue)
                    query = query.Where(x => x.QuestionIdentifier == question.Value);

                if (option.HasValue)
                    query = query.Where(x => x.OptionKey == option.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.PinSequence)
                    .ToList();
            }
        }

        public T[] BindAttemptPins<T>(Expression<Func<QAttemptPin, T>> binder, QAttemptFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).SelectMany(x => x.Pins.Select(y => y));

                query = query
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.PinSequence);

                return query.Select(binder).ToArray();
            }
        }

        public int CountAttemptPins(Guid attempt, Guid? question = null, int? option = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptPins.AsNoTracking().AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (question.HasValue)
                    query = query.Where(x => x.QuestionIdentifier == question.Value);

                if (option.HasValue)
                    query = query.Where(x => x.OptionKey == option.Value);

                return query.Count();
            }
        }

        #endregion

        #region Solutions

        public List<QAttemptSolution> GetAttemptSolutions(Guid attempt, Guid? question = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QAttemptSolutions
                    .AsNoTracking()
                    .AsQueryable()
                    .Where(x => x.AttemptIdentifier == attempt);

                if (question != null)
                    query = query.Where(x => x.QuestionIdentifier == question.Value);

                return query
                    .AsEnumerable()
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.SolutionSequence)
                    .ToList();
            }
        }

        public List<QAttemptSolution> GetAttemptMatchedSolutions(Guid attempt)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptSolutions.AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.AnswerIsMatched)
                    .ToList();
            }
        }

        public QAttemptSolution GetAttemptMatchedSolution(Guid attempt, Guid question)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptSolutions.AsNoTracking()
                    .Where(x => x.AttemptIdentifier == attempt && x.QuestionIdentifier == question && x.AnswerIsMatched)
                    .FirstOrDefault();
            }
        }

        public List<Guid> GetAttemptExistSolutionIds(Guid question)
        {
            using (var db = CreateContext())
            {
                return db.QAttemptSolutions
                    .Where(x => x.QuestionIdentifier == question)
                    .Select(x => x.SolutionIdentifier)
                    .Distinct()
                    .ToList();
            }
        }

        public List<QAttempt> GetAttemptsByDistribution(Guid organizationId, Guid managerUserId)
        {
            using (var db = new InternalDbContext())
            {
                var query =
                    db.QAttempts
                       .Where(a => a.OrganizationIdentifier == organizationId)
                       .Join(
                           db.QCourseEnrollments,
                           a => a.LearnerUserIdentifier,
                           e => e.LearnerUserIdentifier,
                           (a, e) => new { a, e }
                       )
                       .Join(
                           db.TCourseDistributions.Where(d =>
                               d.ManagerUserIdentifier == managerUserId &&
                               d.CourseEnrollmentIdentifier != null
                           ),
                           ae => ae.e.CourseEnrollmentIdentifier,
                           d => d.CourseEnrollmentIdentifier,
                           (ae, d) => ae.a
                       )
                       .Include(x => x.Form)
                       .Include(x => x.LearnerUser)
                       .AsNoTracking();

                return query.ToList();
            }
        }

        #endregion
    }
}