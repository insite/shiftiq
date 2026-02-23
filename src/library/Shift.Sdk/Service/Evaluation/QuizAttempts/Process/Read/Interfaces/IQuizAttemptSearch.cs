using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.QuizAttempts.Read
{
    public interface IQuizAttemptSearch
    {
        int Count(TQuizAttemptFilter filter);
        List<TQuizAttempt> Select(TQuizAttemptFilter filter, params Expression<Func<TQuizAttempt, object>>[] includes);
        TQuizAttempt Select(Guid attemptId, params Expression<Func<TQuizAttempt, object>>[] includes);
        TQuizAttempt SelectLatest(TQuizAttemptFilter filter, params Expression<Func<TQuizAttempt, object>>[] includes);
    }
}
