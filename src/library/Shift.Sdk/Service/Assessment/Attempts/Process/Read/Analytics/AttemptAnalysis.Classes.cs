using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Banks.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Application.Attempts.Read
{
    public partial class AttemptAnalysis
    {
        private class QuestionInfo : IAttemptAnalysisQuestion
        {
            #region Properties

            public Question Question { get; }

            public int AttemptCount { get; }

            public int SuccessCount { get; }

            public double SuccessRate { get; }

            public int NoAnswerCount { get; }

            public decimal NoAnswerRate => AttemptCount == 0 ? 0 : (decimal)NoAnswerCount / AttemptCount;

            public int NoAnswerAverageAttemptScorePercent { get; }

            public double NoAnswerItemTotalCorrelation { get; }

            public double NoAnswerItemRestCoefficient { get; }

            #endregion

            #region Fields

            private AttemptAnalysis _analysis;
            private ReadOnlyCollection<QuestionEntity> _attempts;

            #endregion

            #region Construction

            public QuestionInfo(Question question, AttemptAnalysis analysis)
            {
                Question = question;

                _analysis = analysis;
                _attempts = analysis._attemptQuestionsById.ContainsKey(question.Identifier)
                    ? analysis._attemptQuestionsById[question.Identifier].AsReadOnly()
                    : Array.AsReadOnly(new QuestionEntity[0]);

                AttemptCount = _attempts.Count;

                var itemPoints = new List<double>(_attempts.Count);
                var lostPoints = new List<double>(_attempts.Count);
                var totalPoints = new List<double>(_attempts.Count);
                var differencePoints = new List<double>(_attempts.Count);
                var noAnswerAttemptScore = 0m;
                var noAnswerAttemptCount = 0m;

                for (var i = 0; i < _attempts.Count; i++)
                {
                    var attempt = _attempts[i];

                    if (attempt.AnswerPoints != null && attempt.AnswerPoints > 0)
                        SuccessCount++;

                    var lostPoint = 0m;
                    var pointTotal = (double)(attempt.Attempt.AttemptPoints ?? 0M);

                    if (!attempt.AnswerOptionKey.HasValue)
                    {
                        NoAnswerCount++;

                        lostPoint = question.Points ?? 0;

                        noAnswerAttemptScore += attempt.Attempt.AttemptScore ?? 0;
                        noAnswerAttemptCount++;
                    }

                    itemPoints.Add(0);
                    lostPoints.Add((double)lostPoint);
                    totalPoints.Add(pointTotal);
                    differencePoints.Add(pointTotal);
                }

                SuccessRate = AttemptCount == 0 ? 0 : (double)SuccessCount / AttemptCount;
                NoAnswerAverageAttemptScorePercent = noAnswerAttemptCount == 0 ? 0 : (int)(100M * noAnswerAttemptScore / noAnswerAttemptCount);
                NoAnswerItemTotalCorrelation = (lostPoints.Count <= 1 ? 0 : Calculator.CalculateCorrelation(lostPoints, totalPoints));
                NoAnswerItemRestCoefficient = itemPoints.Count <= 1 ? 0 : Calculator.CalculateCorrelation(itemPoints, differencePoints);
            }

            #endregion

            #region Methods

            public IEnumerable<IAttemptAnalysisOption> GetOptions() =>
                Question.Options.Select(x => new OptionInfo(x, _analysis)).ToArray();

            #endregion
        }

        private class OptionInfo : IAttemptAnalysisOption
        {
            #region Properties

            public Option Option { get; }

            public int AttemptCount { get; }

            public int AnswerCount { get; }

            public decimal AnswerRate { get; }

            public int AverageAttemptScorePercent { get; set; }

            public double ItemTotalCorrelation { get; set; }

            public double ItemRestCoefficient { get; set; }

            #endregion

            #region Fields

            private AttemptAnalysis _analysis;
            private ReadOnlyCollection<OptionEntity> _attempts;

            #endregion

            #region Construction

            public OptionInfo(Option option, AttemptAnalysis analysis)
            {
                Option = option;

                _analysis = analysis;
                _attempts = analysis._attemptOptionsByKey.ContainsKey(option.Number)
                    ? analysis._attemptOptionsByKey[option.Number].AsReadOnly()
                    : Array.AsReadOnly(new OptionEntity[0]);

                AttemptCount = _attempts.Count;

                var itemPoints = new List<double>(_attempts.Count);
                var lostPoints = new List<double>(_attempts.Count);
                var totalPoints = new List<double>(_attempts.Count);
                var differencePoints = new List<double>(_attempts.Count);
                var answeredAttemptsScore = 0m;
                var answeredAttemptsCount = 0;

                for (var i = 0; i < _attempts.Count; i++)
                {
                    var attempt = _attempts[i];

                    if (attempt.AnswerIsSelected == true)
                    {
                        answeredAttemptsScore += attempt.Attempt.AttemptScore ?? 0;
                        answeredAttemptsCount++;
                    }

                    var itemPoint = attempt.AnswerIsSelected == true && option.HasPoints ? (double)option.Points : 0.0;
                    var lostPoint = attempt.AnswerIsSelected == true && !option.HasPoints ? (double)option.Question.Points : 0.0;
                    var pointTotal = (double)(attempt.Attempt.AttemptPoints ?? 0);

                    itemPoints.Add(itemPoint);
                    lostPoints.Add(lostPoint);
                    totalPoints.Add(pointTotal);
                    differencePoints.Add(pointTotal - itemPoint);
                }

                AnswerCount = answeredAttemptsCount;
                AverageAttemptScorePercent = answeredAttemptsCount == 0 ? 0 : (int)(100 * answeredAttemptsScore / answeredAttemptsCount);
                AnswerRate = AttemptCount == 0 ? 0 : (decimal)AnswerCount / AttemptCount;
                ItemTotalCorrelation = option.HasPoints
                    ? (itemPoints.Count <= 1 ? 0 : Calculator.CalculateCorrelation(itemPoints, totalPoints))
                    : (lostPoints.Count <= 1 ? 0 : Calculator.CalculateCorrelation(lostPoints, totalPoints));
                ItemRestCoefficient = itemPoints.Count <= 1
                    ? 0
                    : Calculator.CalculateCorrelation(itemPoints, differencePoints);
            }

            #endregion
        }

        public class AttemptEntity
        {
            public Guid AttemptIdentifier { get; set; }
            public Guid FormIdentifier { get; set; }
            public Guid LearnerUserIdentifier { get; set; }

            public DateTimeOffset? AttemptStarted { get; set; }
            public DateTimeOffset? AttemptSubmitted { get; set; }
            public DateTimeOffset? AttemptGraded { get; set; }

            public int AttemptNumber { get; set; }
            public decimal? AttemptScore { get; set; }
            public decimal? AttemptPoints { get; set; }
            public bool AttemptIsPassing { get; set; }

            public QuestionEntity[] Questions { get; internal set; }
            public OptionEntity[] Options { get; internal set; }

            public static readonly Expression<Func<QAttempt, AttemptEntity>> Binder = LinqExtensions1.Expr((QAttempt x) => new AttemptEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                FormIdentifier = x.FormIdentifier,
                LearnerUserIdentifier = x.LearnerUserIdentifier,
                AttemptStarted = x.AttemptStarted,
                AttemptSubmitted = x.AttemptSubmitted,
                AttemptGraded = x.AttemptGraded,
                AttemptNumber = x.AttemptNumber,
                AttemptScore = x.AttemptScore,
                AttemptPoints = x.AttemptPoints,
                AttemptIsPassing = x.AttemptIsPassing
            });
        }

        public class QuestionEntity
        {
            public Guid AttemptIdentifier { get; set; }
            public Guid QuestionIdentifier { get; set; }
            public Guid? ParentQuestionIdentifier { get; set; }

            public int QuestionSequence { get; set; }
            public decimal? QuestionPoints { get; set; }
            public decimal? AnswerPoints { get; set; }
            public int? AnswerOptionKey { get; set; }

            public Guid? CompetencyAreaIdentifier { get; set; }
            public string CompetencyAreaLabel { get; set; }
            public string CompetencyAreaCode { get; set; }
            public string CompetencyAreaTitle { get; set; }

            public Guid? CompetencyItemIdentifier { get; set; }
            public string CompetencyItemLabel { get; set; }
            public string CompetencyItemCode { get; set; }
            public string CompetencyItemTitle { get; set; }

            public AttemptEntity Attempt { get; internal set; }
            public QuestionEntity Parent { get; internal set; }
            public QuestionEntity[] SubQuestions { get; internal set; }

            public static readonly Expression<Func<QAttemptQuestion, QuestionEntity>> Binder = LinqExtensions1.Expr((QAttemptQuestion x) => new QuestionEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                QuestionPoints = x.QuestionPoints,
                ParentQuestionIdentifier = x.ParentQuestionIdentifier,
                AnswerPoints = x.AnswerPoints,
                AnswerOptionKey = x.AnswerOptionKey,
                CompetencyAreaIdentifier = x.CompetencyAreaIdentifier,
                CompetencyAreaLabel = x.CompetencyAreaLabel,
                CompetencyAreaCode = x.CompetencyAreaCode,
                CompetencyAreaTitle = x.CompetencyAreaTitle,
                CompetencyItemIdentifier = x.CompetencyItemIdentifier,
                CompetencyItemLabel = x.CompetencyItemLabel,
                CompetencyItemCode = x.CompetencyItemCode,
                CompetencyItemTitle = x.CompetencyItemTitle,
            });
        }

        public class OptionEntity
        {
            public Guid AttemptIdentifier { get; set; }
            public Guid QuestionIdentifier { get; set; }
            public int OptionKey { get; set; }

            public int QuestionSequence { get; set; }
            public int OptionSequence { get; set; }
            public decimal OptionPoints { get; set; }
            public bool? AnswerIsSelected { get; set; }

            public Guid? CompetencyAreaIdentifier { get; set; }
            public string CompetencyAreaLabel { get; set; }
            public string CompetencyAreaCode { get; set; }
            public string CompetencyAreaTitle { get; set; }

            public Guid? CompetencyItemIdentifier { get; set; }
            public string CompetencyItemLabel { get; set; }
            public string CompetencyItemCode { get; set; }
            public string CompetencyItemTitle { get; set; }

            public AttemptEntity Attempt { get; internal set; }

            public static readonly Expression<Func<QAttemptOption, OptionEntity>> Binder = LinqExtensions1.Expr((QAttemptOption x) => new OptionEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                OptionKey = x.OptionKey,
                OptionSequence = x.OptionSequence,
                OptionPoints = x.OptionPoints,
                AnswerIsSelected = x.AnswerIsSelected,
                CompetencyAreaIdentifier = x.CompetencyAreaIdentifier,
                CompetencyAreaLabel = x.CompetencyAreaLabel,
                CompetencyAreaCode = x.CompetencyAreaCode,
                CompetencyAreaTitle = x.CompetencyAreaTitle,
                CompetencyItemIdentifier = x.CompetencyItemIdentifier,
                CompetencyItemLabel = x.CompetencyItemLabel,
                CompetencyItemCode = x.CompetencyItemCode,
                CompetencyItemTitle = x.CompetencyItemTitle,
            });
        }

        public class Settings
        {
            public IAttemptSearch AttemptSearch { get; }
            public IBankSearch BankSearch { get; }

            public QAttemptFilter Filter { get; set; }

            public bool IncludeOnlyFirstAttempt { get; set; }

            public Expression<Func<QAttempt, AttemptEntity>> AttemptEntityBinder { get; set; } = AttemptEntity.Binder;
            public Expression<Func<QAttemptQuestion, QuestionEntity>> QuestionEntityBinder { get; set; } = QuestionEntity.Binder;
            public Expression<Func<QAttemptOption, OptionEntity>> OptionEntityBinder { get; set; } = OptionEntity.Binder;

            public Settings(IAttemptSearch attemptSearch, IBankSearch bankSearch)
            {
                AttemptSearch = attemptSearch;
                BankSearch = bankSearch;
            }
        }

        private class AttemptQuestionLoader
        {
            private Dictionary<Guid, List<QuestionEntity>> _attemptQuestionsByAttemptId = new Dictionary<Guid, List<QuestionEntity>>();
            private Dictionary<(Guid, Guid), List<QuestionEntity>> _attemptQuestionsByParentId = new Dictionary<(Guid, Guid), List<QuestionEntity>>();
            private Dictionary<Guid, List<OptionEntity>> _attemptOptionsByAttemptId = new Dictionary<Guid, List<OptionEntity>>();

            private AttemptQuestionLoader()
            {

            }

            internal static AttemptQuestionLoader LoadData(Settings settings)
            {
                var result = new AttemptQuestionLoader();

                var questions = settings.AttemptSearch.BindAttemptQuestions(settings.QuestionEntityBinder, settings.Filter);
                foreach (var q in questions.OrderBy(x => x.QuestionSequence))
                {
                    result._attemptQuestionsByAttemptId.GetOrAdd(q.AttemptIdentifier, () => new List<QuestionEntity>()).Add(q);

                    if (q.ParentQuestionIdentifier.HasValue)
                        result._attemptQuestionsByParentId.GetOrAdd((q.AttemptIdentifier, q.ParentQuestionIdentifier.Value), () => new List<QuestionEntity>()).Add(q);
                }

                var options = settings.AttemptSearch.BindAttemptOptions(settings.OptionEntityBinder, settings.Filter);
                result._attemptOptionsByAttemptId = options
                    .OrderBy(x => x.QuestionSequence)
                    .ThenBy(x => x.OptionSequence)
                    .GroupBy(x => x.AttemptIdentifier)
                    .ToDictionary(x => x.Key, x => x.ToList());

                return result;
            }

            internal void BindAttempt(AttemptAnalysis analysis, AttemptEntity attempt, HashSet<MultiKey<Guid, Guid>> bankFormQuestions)
            {
                var attemptQuestions = _attemptQuestionsByAttemptId.GetOrDefault(attempt.AttemptIdentifier).EmptyIfNull();
                var attemptOptions = _attemptOptionsByAttemptId.GetOrDefault(attempt.AttemptIdentifier).EmptyIfNull();
                var validQuestions = new HashSet<Guid>();

                for (var i = 0; i < attemptQuestions.Count; i++)
                {
                    var question = attemptQuestions[i];

                    if (bankFormQuestions.Contains(new MultiKey<Guid, Guid>(attempt.FormIdentifier, question.QuestionIdentifier)))
                    {
                        question.Attempt = attempt;
                        question.SubQuestions = _attemptQuestionsByParentId.GetOrDefault((question.AttemptIdentifier, question.QuestionIdentifier)).EmptyIfNull().ToArray();
                        analysis._attemptQuestionsById.GetOrAdd(question.QuestionIdentifier, () => new List<QuestionEntity>()).Add(question);

                        validQuestions.Add(question.QuestionIdentifier);
                        foreach (var subQuestion in question.SubQuestions)
                        {
                            subQuestion.Parent = question;
                            validQuestions.Add(subQuestion.QuestionIdentifier);
                        }
                    }
                    else
                    {
                        attemptQuestions.RemoveAt(i--);
                    }
                }

                for (var i = 0; i < attemptOptions.Count; i++)
                {
                    var option = attemptOptions[i];

                    if (validQuestions.Contains(option.QuestionIdentifier))
                    {
                        option.Attempt = attempt;
                        analysis._attemptOptionsByKey.GetOrAdd(option.OptionKey, () => new List<OptionEntity>()).Add(option);
                    }
                    else
                    {
                        attemptOptions.RemoveAt(i--);
                    }
                }

                attempt.Questions = attemptQuestions.ToArray();
                attempt.Options = attemptOptions.ToArray();
            }
        }
    }
}
