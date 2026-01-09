using System;

using InSite.Domain.Attempts;
using InSite.Domain.Banks;

namespace InSite.Application.Attempts.Read
{
    public interface IInstructorAttemptStore
    {
        void DeleteAttempt(Guid id);
        void DeleteAttempt(AttemptVoided e);

        void InsertAttempt(AttemptImported e, Form form);
        void InsertAttempt(AttemptStarted2 e, Form form);
        void InsertAttempt(AttemptStarted3 e);

        void UpdateAttempt(AttemptSubmitted e);
        void UpdateAttempt(AttemptPinged e);
        void UpdateAttempt(AttemptResumed e);
        void UpdateAttempt(AttemptCommentPosted e);
        void UpdateAttempt(ComposedQuestionAnswered e);
        void UpdateAttempt(ComposedQuestionAttemptStarted e);
        void UpdateAttempt(MatchingQuestionAnswered e);
        void UpdateAttempt(MultipleChoiceQuestionAnswered e);
        void UpdateAttempt(MultipleCorrectQuestionAnswered e);
        void UpdateAttempt(BooleanTableQuestionAnswered e);
        void UpdateAttempt(TrueOrFalseQuestionAnswered e);
        void UpdateAttempt(HotspotQuestionAnswered e);
        void UpdateAttempt(OrderingQuestionAnswered e);
        void UpdateAttempt(AttemptSectionSwitched e);
        void UpdateAttempt(AttemptQuestionSwitched e);

        void UpdateAttempt(AttemptGraded e);
        void UpdateAttempt(AttemptAnalyzed e);
        void UpdateAttempt(AttemptGradedDateChanged e);
        void UpdateAttempt(AttemptFixed e);
        void UpdateAttempt(AttemptTagged e);
        void UpdateAttempt(QuestionVoided e);
        void UpdateAttempt(QuestionRegraded e, Form form);
        void UpdateAttempt(ComposedQuestionScored e);
        void UpdateAttempt(AttemptGradingAssessorAssigned e);
        void UpdateAttempt(ScoreCalculated e);
        void UpdateAttempt(AttemptRubricPointsUpdated e);
        void UpdateAttempt(AttemptRubricChanged e);
        void UpdateAttempt(AttemptQuestionRubricChanged e);
    }
}
