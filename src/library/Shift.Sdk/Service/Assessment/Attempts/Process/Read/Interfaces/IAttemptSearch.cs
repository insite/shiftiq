using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Application.Contents.Read;

using Shift.Common;

namespace InSite.Application.Attempts.Read
{
    public interface IAttemptSearch
    {
        // Comments

        QComment GetQAttemptComment(Guid attemptId, Guid questionIdentifier, Guid authorIdentifier);
        List<QComment> GetQAttemptComments(Guid attemptId, Guid authorIdentifier);
        List<VComment> GetVAttemptComments(Guid attempt);
        List<QAttemptCommentExtended> GetVAttemptComments(QAttemptFilter filter);

        List<QAttemptCommentaryItem> SelectExaminationFeedback(QAttemptCommentaryFilter filter);
        List<QuestionCommentaryItem> SelectQuestionFeedbackForAnalysis(Guid questionId);
        int CountExaminationFeedback(QAttemptCommentaryFilter filter);

        // Sections

        QAttemptSection GetAttemptSection(Guid attempt, int section);
        List<QAttemptSection> GetAttemptSections(Guid attempt);

        // Questions

        Guid[] GetExistsQuestionIdentifiers(IEnumerable<Guid> questionIds);
        List<QAttemptQuestion> GetAttemptQuestions(Guid attempt);
        List<string> GetAttemptQuestionTypes(Guid attempt);
        List<QAttemptQuestion> GetAttemptQuestionsBySequence(Guid attempt, int[] sequence);
        QAttemptQuestion GetAttemptQuestion(Guid attempt, int sequence);
        List<QAttemptQuestion> GetAttemptQuestions(QAttemptQuestionFilter filter);
        List<QAttemptQuestion> GetAttemptQuestions(Guid attempt, int? sectionIndex);
        List<Domain.Attempts.AnswerState> GetAttemptQuestionsByLearner(Guid learner, Guid[] forms);
        T[] BindAttemptQuestions<T>(Expression<Func<QAttemptQuestion, T>> binder, QAttemptFilter filter);
        int CountAttemptQuestions(Guid attempt);

        // Options

        List<int> GetAttemptExistOptionKeys(Guid question);
        List<QAttemptOption> GetAttemptOptions(Guid attempt, Guid? question = null);
        List<QAttemptOption> GetAttemptOptions(Guid attempt, Guid[] questions);
        List<QAttemptOption> GetAttemptOptions(QAttemptFilter filter);
        T[] BindAttemptOptions<T>(Expression<Func<QAttemptOption, T>> binder, Guid attempt);
        T[] BindAttemptOptions<T>(Expression<Func<QAttemptOption, T>> binder, QAttemptFilter filter);
        int? GetAttemptOptionKeyBySequence(Guid attempt, Guid question, int optionSequence);
        List<int> GetAttemptOptionKeysBySequence(Guid attempt, Guid question, int[] optionSequence);

        // Matches

        List<QAttemptMatch> GetAttemptMatches(Guid attempt, Guid? question);
        T[] BindAttemptMatches<T>(Expression<Func<QAttemptMatch, T>> binder, QAttemptFilter filter);

        // Attempts

        QAttempt GetAttempt(Guid attempt, params Expression<Func<QAttempt, object>>[] includes);
        List<QAttempt> GetAttempts(QAttemptFilter filter, params Expression<Func<QAttempt, object>>[] includes);
        List<QAttempt> GetAttempts(Guid form, Guid user, params Expression<Func<QAttempt, object>>[] includes);
        List<QAttempt> GetAttemptsByEvent(Guid @event, string filterText = null, Paging paging = null, bool includeQuestions = false, bool includeEvent = false);
        T[] BindAttempts<T>(Expression<Func<QAttempt, T>> binder, QAttemptFilter filter);
        int CountAttempts(QAttemptFilter filter);
        int CountAttempts(Expression<Func<QAttempt, bool>> filter);

        List<string> GetAttemptTags(Guid organizationId);

        Guid[] GetOrphanAttempts();

        // Pins

        List<QAttemptPin> GetAttemptPins(Guid attempt, Guid? question, int? option);
        T[] BindAttemptPins<T>(Expression<Func<QAttemptPin, T>> binder, QAttemptFilter filter);
        int CountAttemptPins(Guid attempt, Guid? question, int? option);

        // Solutions

        List<Guid> GetAttemptExistSolutionIds(Guid question);
        List<QAttemptSolution> GetAttemptSolutions(Guid attempt, Guid? question = null);
        List<QAttemptSolution> GetAttemptMatchedSolutions(Guid attempt);
        QAttemptSolution GetAttemptMatchedSolution(Guid attempt, Guid question);
        List<QAttempt> GetAttemptsByDistribution(Guid organizationId, Guid managerUserId);
    }
}