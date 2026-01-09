using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Application.Attempts.Read
{
    public partial class AttemptAnalysis
    {
        #region Classes


        #endregion

        #region Properties

        public ReadOnlyCollection<AttemptEntity> Attempts => _attempts.AsReadOnly();

        public ReadOnlyDictionary<Guid, Form> Forms => _bankFormsReadOnly;

        public ReadOnlyDictionary<Guid, Question> Questions => _bankQuestionsByIdReadOnly;

        public int QuestionCount => _bankQuestionsById.Count;

        public bool HasData => _attempts.Count > 0;

        #endregion

        #region Fields

        private List<AttemptEntity> _attempts;
        private Dictionary<Guid, List<AttemptEntity>> _attemptsByFormId;
        private Dictionary<Guid, List<QuestionEntity>> _attemptQuestionsById;
        private Dictionary<int, List<OptionEntity>> _attemptOptionsByKey;
        private Dictionary<Guid, Form> _bankForms;
        private ReadOnlyDictionary<Guid, Form> _bankFormsReadOnly;
        private Dictionary<Guid, Question> _bankQuestionsById;
        private ReadOnlyDictionary<Guid, Question> _bankQuestionsByIdReadOnly;

        #endregion

        #region Construction

        private AttemptAnalysis()
        {

        }

        #endregion

        #region Methods (initialization)

        public static AttemptAnalysis Empty()
        {
            var result = new AttemptAnalysis();

            result._attempts = new List<AttemptEntity>();
            result._attemptsByFormId = new Dictionary<Guid, List<AttemptEntity>>();
            result._attemptQuestionsById = new Dictionary<Guid, List<QuestionEntity>>();
            result._attemptOptionsByKey = new Dictionary<int, List<OptionEntity>>();
            result._bankForms = new Dictionary<Guid, Form>();
            result._bankFormsReadOnly = new ReadOnlyDictionary<Guid, Form>(result._bankForms);
            result._bankQuestionsById = new Dictionary<Guid, Question>();
            result._bankQuestionsByIdReadOnly = new ReadOnlyDictionary<Guid, Question>(result._bankQuestionsById);

            return result;
        }

        public static AttemptAnalysis Create(Settings settings)
        {
            var result = new AttemptAnalysis();

            LoadAttempts(result, settings);
            LoadBankForms(result, settings, out var bankFormQuestions);
            RemoveAttemptsWithoutForms(result);

            if (settings.IncludeOnlyFirstAttempt)
            {
                var excludedAttempts = new HashSet<Guid>();

                result._attempts = result._attempts.GroupBy(x => new { x.FormIdentifier, x.LearnerUserIdentifier }).Select(group =>
                {
                    AttemptEntity attempt = null;

                    foreach (var entity in group.OrderBy(y => y.AttemptStarted).ThenBy(y => y.AttemptGraded))
                    {
                        if (attempt == null)
                            attempt = entity;
                        else
                            excludedAttempts.Add(entity.AttemptIdentifier);
                    }

                    return attempt;
                }).ToList();
            }

            LoadAttemptQuestions(result, settings, bankFormQuestions);

            return result;
        }

        private static void LoadAttempts(AttemptAnalysis analysis, Settings settings)
        {
            analysis._attempts = settings.AttemptSearch
                .BindAttempts(settings.AttemptEntityBinder, settings.Filter)
                .OrderByDescending(x => x.AttemptStarted).ToList();
            analysis._attemptsByFormId = new Dictionary<Guid, List<AttemptEntity>>();
            {
                for (var i = 0; i < analysis._attempts.Count; i++)
                {
                    var attempt = analysis._attempts[i];

                    if (!analysis._attemptsByFormId.TryGetValue(attempt.FormIdentifier, out var formAttempts))
                        analysis._attemptsByFormId.Add(attempt.FormIdentifier, formAttempts = new List<AttemptEntity>());

                    formAttempts.Add(attempt);
                }
            }
        }

        private static void LoadBankForms(AttemptAnalysis analysis, Settings settings, out HashSet<MultiKey<Guid, Guid>> bankFormQuestions)
        {
            bankFormQuestions = new HashSet<MultiKey<Guid, Guid>>();

            analysis._bankForms = new Dictionary<Guid, Form>();
            analysis._bankFormsReadOnly = new ReadOnlyDictionary<Guid, Form>(analysis._bankForms);
            analysis._bankQuestionsById = new Dictionary<Guid, Question>();
            analysis._bankQuestionsByIdReadOnly = new ReadOnlyDictionary<Guid, Question>(analysis._bankQuestionsById);

            var banks = new Dictionary<Guid, BankState>();
            var qForms = settings.BankSearch.GetForms(analysis._attemptsByFormId.Keys);

            for (var i = 0; i < qForms.Count; i++)
            {
                var qForm = qForms[i];
                if (!banks.TryGetValue(qForm.BankIdentifier, out var bank))
                    banks.Add(qForm.BankIdentifier, bank = settings.BankSearch.GetBankState(qForm.BankIdentifier));

                if (bank == null)
                    continue;

                var bForm = bank?.FindForm(qForm.FormIdentifier);
                if (bForm == null)
                    continue;

                analysis._bankForms.Add(bForm.Identifier, bForm);

                foreach (var bQuestion in bForm.GetQuestions())
                {
                    bankFormQuestions.Add(new MultiKey<Guid, Guid>(bForm.Identifier, bQuestion.Identifier));

                    if (!analysis._bankQuestionsById.ContainsKey(bQuestion.Identifier))
                        analysis._bankQuestionsById.Add(bQuestion.Identifier, bQuestion);
                }
            }
        }

        private static void RemoveAttemptsWithoutForms(AttemptAnalysis analysis)
        {
            var existFormIds = analysis._attemptsByFormId.Keys.ToArray();
            var removeAttempt = new HashSet<Guid>();

            for (var i = 0; i < existFormIds.Length; i++)
            {
                var formId = existFormIds[i];
                if (analysis._bankForms.ContainsKey(formId))
                    continue;

                var formAttempts = analysis._attemptsByFormId[formId];
                for (var j = 0; j < formAttempts.Count; j++)
                    removeAttempt.Add(formAttempts[i].AttemptIdentifier);

                analysis._attemptsByFormId.Remove(formId);
            }

            if (removeAttempt.Count > 0)
                analysis._attempts = analysis._attempts.Where(x => removeAttempt.Contains(x.AttemptIdentifier)).ToList();
        }

        private static void LoadAttemptQuestions(AttemptAnalysis analysis, Settings settings, HashSet<MultiKey<Guid, Guid>> bankFormQuestions)
        {
            analysis._attemptQuestionsById = new Dictionary<Guid, List<QuestionEntity>>();
            analysis._attemptOptionsByKey = new Dictionary<int, List<OptionEntity>>();

            var dataLoader = AttemptQuestionLoader.LoadData(settings);

            for (var i = 0; i < analysis._attempts.Count; i++)
            {
                var attempt = analysis._attempts[i];

                dataLoader.BindAttempt(analysis, attempt, bankFormQuestions);

                if (attempt.Questions.Length == 0)
                {
                    analysis._attempts.RemoveAt(i--);

                    var formAttempts = analysis._attemptsByFormId[attempt.FormIdentifier];

                    formAttempts.RemoveAll(x => x.AttemptIdentifier == attempt.AttemptIdentifier);

                    if (formAttempts.Count == 0)
                    {
                        analysis._attemptsByFormId.Remove(attempt.FormIdentifier);
                        analysis._bankForms.Remove(attempt.FormIdentifier);
                    }
                }
            }
        }

        public AttemptAnalysis FilterAttempt(Func<AttemptEntity, bool> filter)
        {
            var result = new AttemptAnalysis();
            result._attempts = new List<AttemptEntity>();
            result._attemptsByFormId = new Dictionary<Guid, List<AttemptEntity>>();
            result._attemptQuestionsById = new Dictionary<Guid, List<QuestionEntity>>();
            result._attemptOptionsByKey = new Dictionary<int, List<OptionEntity>>();
            result._bankForms = new Dictionary<Guid, Form>();
            result._bankFormsReadOnly = new ReadOnlyDictionary<Guid, Form>(result._bankForms);
            result._bankQuestionsById = new Dictionary<Guid, Question>();
            result._bankQuestionsByIdReadOnly = new ReadOnlyDictionary<Guid, Question>(result._bankQuestionsById);

            for (var i = 0; i < _attempts.Count; i++)
            {
                var attempt = _attempts[i];
                if (!filter(attempt))
                    continue;

                result._attempts.Add(attempt);

                {
                    var formId = attempt.FormIdentifier;

                    if (!result._attemptsByFormId.TryGetValue(formId, out var formAttempts))
                    {
                        result._attemptsByFormId.Add(formId, formAttempts = new List<AttemptEntity>());

                        if (_bankForms.ContainsKey(formId))
                            result._bankForms.Add(formId, _bankForms[formId]);
                    }

                    formAttempts.Add(attempt);
                }

                foreach (var attemptQuestion in attempt.Questions)
                {
                    var questionId = attemptQuestion.QuestionIdentifier;

                    if (!result._attemptQuestionsById.TryGetValue(questionId, out var qAttempts))
                    {
                        result._attemptQuestionsById.Add(questionId, qAttempts = new List<QuestionEntity>());

                        if (_bankQuestionsById.ContainsKey(questionId))
                            result._bankQuestionsById.Add(questionId, _bankQuestionsById[questionId]);
                    }

                    qAttempts.Add(attemptQuestion);
                }

                foreach (var attemptOption in attempt.Options)
                {
                    var optionKey = attemptOption.OptionKey;

                    if (!result._attemptOptionsByKey.TryGetValue(optionKey, out var oAttempts))
                        result._attemptOptionsByKey.Add(optionKey, oAttempts = new List<OptionEntity>());

                    oAttempts.Add(attemptOption);
                }
            }

            return result;
        }

        public AttemptAnalysis FilterQuestion(Func<QuestionEntity, bool> filter)
        {
            var result = new AttemptAnalysis();
            result._attempts = new List<AttemptEntity>();
            result._attemptsByFormId = new Dictionary<Guid, List<AttemptEntity>>();
            result._attemptQuestionsById = new Dictionary<Guid, List<QuestionEntity>>();
            result._attemptOptionsByKey = new Dictionary<int, List<OptionEntity>>();
            result._bankForms = new Dictionary<Guid, Form>();
            result._bankFormsReadOnly = new ReadOnlyDictionary<Guid, Form>(result._bankForms);
            result._bankQuestionsById = new Dictionary<Guid, Question>();
            result._bankQuestionsByIdReadOnly = new ReadOnlyDictionary<Guid, Question>(result._bankQuestionsById);

            foreach (var srcAttempt in _attempts)
            {
                var questions = new List<QuestionEntity>();
                var options = new List<OptionEntity>();

                var destAttempt = new AttemptEntity();

                foreach (var srcQuestion in srcAttempt.Questions)
                {
                    if (!filter(srcQuestion))
                        continue;

                    var questionId = srcQuestion.QuestionIdentifier;
                    var destQuestion = new QuestionEntity { Attempt = destAttempt };

                    srcQuestion.ShallowCopyTo(destQuestion);

                    if (!result._attemptQuestionsById.TryGetValue(questionId, out var qAttempts))
                    {
                        result._attemptQuestionsById.Add(questionId, qAttempts = new List<QuestionEntity>());

                        if (_bankQuestionsById.ContainsKey(questionId))
                            result._bankQuestionsById.Add(questionId, _bankQuestionsById[questionId]);
                    }

                    qAttempts.Add(destQuestion);
                    questions.Add(destQuestion);
                }

                if (questions.Count == 0)
                    continue;

                var questionIds = new HashSet<Guid>(questions.Select(x => x.QuestionIdentifier));

                foreach (var srcOption in srcAttempt.Options)
                {
                    if (!questionIds.Contains(srcOption.QuestionIdentifier))
                        continue;

                    var optionKey = srcOption.OptionKey;
                    var destOption = new OptionEntity { Attempt = destAttempt };

                    srcOption.ShallowCopyTo(destOption);

                    if (!result._attemptOptionsByKey.TryGetValue(optionKey, out var oAttempts))
                        result._attemptOptionsByKey.Add(optionKey, oAttempts = new List<OptionEntity>());

                    oAttempts.Add(destOption);
                    options.Add(destOption);
                }

                srcAttempt.ShallowCopyTo(destAttempt);

                destAttempt.Questions = questions.ToArray();
                destAttempt.Options = options.ToArray();

                result._attempts.Add(destAttempt);

                {
                    if (!result._attemptsByFormId.TryGetValue(destAttempt.FormIdentifier, out var formAttempts))
                    {
                        result._attemptsByFormId.Add(destAttempt.FormIdentifier, formAttempts = new List<AttemptEntity>());

                        if (_bankForms.ContainsKey(destAttempt.FormIdentifier))
                            result._bankForms.Add(destAttempt.FormIdentifier, _bankForms[destAttempt.FormIdentifier]);
                    }

                    formAttempts.Add(destAttempt);
                }
            }

            return result;
        }

        #endregion

        #region Methods (analysis)

        public IAttemptAnalysisQuestion[] GetQuestionAnalysis() =>
            _bankQuestionsById.Select(kv => new QuestionInfo(kv.Value, this)).ToArray();

        public double CalculateCronbachAlpha()
        {
            var scores = new List<List<double>>(_attempts.Count);

            foreach (var attempt in _attempts)
            {
                var questionsById = new Dictionary<Guid, QuestionEntity>();
                foreach (var q in attempt.Questions)
                {
                    if (q.SubQuestions != null && q.SubQuestions.Length > 0)
                    {
                        foreach (var sq in q.SubQuestions)
                            questionsById.Add(sq.QuestionIdentifier, sq);
                    }
                    else
                        questionsById.Add(q.QuestionIdentifier, q);
                }

                var questionScores = new Dictionary<Guid, List<double>>();

                foreach (var ro in attempt.Options.Where(x => x.AnswerIsSelected == true))
                {
                    if (!questionScores.ContainsKey(ro.QuestionIdentifier))
                        questionScores.Add(ro.QuestionIdentifier, new List<double>());

                    var question = questionsById[ro.QuestionIdentifier];

                    questionScores[ro.QuestionIdentifier].Add(question.QuestionPoints == 0 ? 0D : (double)ro.OptionPoints / (double)question.QuestionPoints);
                }

                var row = new List<double>(attempt.Questions.Length);

                foreach (var question in attempt.Questions.OrderBy(x => x.QuestionSequence))
                {
                    var score = questionScores.TryGetValue(question.QuestionIdentifier, out var scoreList) && scoreList.Count > 0
                        ? scoreList.Average()
                        : 0.0;

                    row.Add(score);
                }

                scores.Add(row);
            }

            return Calculator.CalculateCronbachAlpha(scores);
        }

        #endregion
    }
}
