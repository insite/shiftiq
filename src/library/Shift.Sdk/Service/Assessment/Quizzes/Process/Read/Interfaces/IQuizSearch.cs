using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Quizzes.Read
{
    public interface IQuizSearch
    {
        int Count(TQuizFilter filter);
        List<TQuiz> Select(TQuizFilter filter, params Expression<Func<TQuiz, object>>[] includes);
        TQuiz Select(Guid quizId, params Expression<Func<TQuiz, object>>[] includes);
    }
}
