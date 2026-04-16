using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Attempts.Read;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Attempts.Utilities
{
    public static class AttemptReportHelper
    {
        internal class ExportQuestionEntity : AttemptAnalysis.QuestionEntity
        {
            public int? AnswerOptionSequence { get; set; }
            public string QuestionType { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
            public string QuestionCalculationMethod { get; set; }
            public Guid? AnswerFileIdentifier { get; set; }

            public Guid? QuestionSetId { get; set; }
            public string QuestionSetName { get; set; }

            public static new readonly Expression<Func<QAttemptQuestion, AttemptAnalysis.QuestionEntity>> Binder = LinqExtensions1.Expr<QAttemptQuestion, AttemptAnalysis.QuestionEntity>(x => new ExportQuestionEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                ParentQuestionIdentifier = x.ParentQuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                QuestionPoints = x.QuestionPoints,
                AnswerPoints = x.AnswerPoints,
                AnswerOptionKey = x.AnswerOptionKey,

                QuestionType = x.QuestionType,
                AnswerOptionSequence = x.AnswerOptionSequence,
                QuestionText = x.QuestionText,
                AnswerText = x.AnswerText,
                QuestionCalculationMethod = x.QuestionCalculationMethod,
                AnswerFileIdentifier = x.AnswerFileIdentifier
            });
        }

        internal class ExportOptionEntity : AttemptAnalysis.OptionEntity
        {
            public string OptionText { get; set; }
            public bool? OptionIsTrue { get; set; }
            public int? OptionAnswerSequence { get; set; }

            public static new readonly Expression<Func<QAttemptOption, AttemptAnalysis.OptionEntity>> Binder = LinqExtensions1.Expr<QAttemptOption, AttemptAnalysis.OptionEntity>(x => new ExportOptionEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                OptionKey = x.OptionKey,
                OptionSequence = x.OptionSequence,
                OptionPoints = x.OptionPoints,
                AnswerIsSelected = x.AnswerIsSelected,
                OptionAnswerSequence = x.OptionAnswerSequence,

                OptionText = x.OptionText,
                OptionIsTrue = x.OptionIsTrue
            });
        }

        internal class ExportData
        {
            public AttemptAnalysis.AttemptEntity[] Attempts { get; }

            private Dictionary<Guid, ExportQuestionEntity[]> _questions;
            private Dictionary<MultiKey<Guid, Guid>, List<ExportOptionEntity>> _optionsByQuestionId;

            public ExportData(AttemptAnalysis analysis)
            {
                _questions = new Dictionary<Guid, ExportQuestionEntity[]>();
                _optionsByQuestionId = new Dictionary<MultiKey<Guid, Guid>, List<ExportOptionEntity>>();

                foreach (var attempt in analysis.Attempts)
                {
                    var questions = attempt.Questions.Cast<ExportQuestionEntity>().ToArray();

                    foreach (var attemptQuestion in questions)
                    {
                        if (!analysis.Questions.TryGetValue(attemptQuestion.QuestionIdentifier, out var bankQuestion))
                            continue;

                        var set = bankQuestion.Set;

                        attemptQuestion.QuestionSetId = set.Identifier;
                        attemptQuestion.QuestionSetName = set.Name;

                        foreach (ExportQuestionEntity subQuestion in attemptQuestion.SubQuestions)
                        {
                            subQuestion.QuestionSetId = set.Identifier;
                            subQuestion.QuestionSetName = set.Name;
                        }
                    }

                    _questions.Add(attempt.AttemptIdentifier, questions);

                    MultiKey<Guid, Guid> questionKey = null;
                    List<ExportOptionEntity> questionOptions = null;

                    foreach (ExportOptionEntity option in attempt.Options.OrderBy(x => x.AttemptIdentifier).ThenBy(x => x.QuestionIdentifier).ThenBy(x => x.OptionSequence))
                    {
                        if (questionKey == null || questionKey.Key1 != option.AttemptIdentifier || questionKey.Key2 != option.QuestionIdentifier)
                            _optionsByQuestionId.Add(
                                questionKey = new MultiKey<Guid, Guid>(option.AttemptIdentifier, option.QuestionIdentifier),
                                questionOptions = new List<ExportOptionEntity>()
                            );

                        questionOptions.Add(option);
                    }
                }

                Attempts = analysis.Attempts.OrderBy(x => x.AttemptStarted).ThenBy(x => x.AttemptGraded).ToArray();
            }

            internal ExportQuestionEntity[] GetAttemptQuestions(Guid attemptId)
            {
                return _questions.TryGetValue(attemptId, out var value) ? value : new ExportQuestionEntity[0];
            }

            internal List<ExportOptionEntity> GetQuestionOptions(Guid attemptId, Guid questionId)
            {
                var key = new MultiKey<Guid, Guid>(attemptId, questionId);

                return _optionsByQuestionId.TryGetValue(key, out var value) ? value : new List<ExportOptionEntity>();
            }

            internal IEnumerable<ExportQuestionEntity> EnumerateAllQuestions()
            {
                foreach (var attempt in Attempts)
                {
                    var questions = GetAttemptQuestions(attempt.AttemptIdentifier);

                    foreach (var question in questions.OrderBy(q => q.QuestionSequence).ThenBy(q => q.AnswerOptionSequence))
                    {
                        yield return question;

                        var subQuestions = question.SubQuestions.Cast<ExportQuestionEntity>();
                        foreach (var subQuestion in subQuestions.OrderBy(q => q.QuestionSequence).ThenBy(q => q.AnswerOptionSequence))
                            yield return subQuestion;
                    }
                }
            }
        }
    }
}
