using System;

namespace InSite.Application.QuizAttempts.Read
{
    public interface IQuizAttemptStore
    {
        void Insert(TQuizAttempt attempt);
        void Update(TQuizAttempt attempt);
        bool Delete(Guid attemptId);
        bool DeleteByQuizId(Guid quizId);
    }
}
