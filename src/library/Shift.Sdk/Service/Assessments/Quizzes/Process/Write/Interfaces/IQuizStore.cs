using System;

namespace InSite.Application.Quizzes.Read
{
    public interface IQuizStore
    {
        void Insert(TQuiz quiz);
        void Update(TQuiz quiz);
        bool Delete(Guid quizId);
    }
}
