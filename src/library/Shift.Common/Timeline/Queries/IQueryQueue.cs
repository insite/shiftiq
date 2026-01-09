using System;

namespace Shift.Common.Timeline.Queries
{
    public interface IQueryQueue
    {
        TResult Send<TResult>(IQuery<TResult> query);
        void Subscribe<TQuery, TResult>(Func<TQuery, TResult> handle) where TQuery : IQuery<TResult>;
    }
}