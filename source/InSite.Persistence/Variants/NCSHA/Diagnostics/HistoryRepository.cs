using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class HistoryRepository
    {
        #region Classes

        private class ReadHelper : ReadHelper<History>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<History>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Histories.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Insert

        public static void Insert(Guid user, string userName, string userEmail, NcshaHistoryEvent @event)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (var db = new InternalDbContext())
            {
                var h = new History
                {
                    UserId = user,
                    UserName = userName,
                    UserEmail = userEmail,
                    EventType = @event.GetType().FullName,
                    EventData = JsonConvert.SerializeObject(@event)
                };
                db.Histories.Add(h);
                db.SaveChanges();
            }
        }

        #endregion

        #region Select

        public static T[] Bind<T>(
            Expression<Func<History, T>> binder,
            HistoryFilter filter)
        {
            return ReadHelper.Instance.Bind(
                (IQueryable<History> q) => q.Select(binder),
                (IQueryable<History> q) => q.ApplyFilter(filter),
                filter.Paging,
                filter.OrderBy,
                null);
        }

        public static int Count(HistoryFilter filter)
        {
            return ReadHelper.Instance.Count((IQueryable<History> q) => q.ApplyFilter(filter));
        }

        public static T[] Distinct<T>(Expression<Func<History, T>> binder, Expression<Func<History, bool>> filter, string modelSort = null)
        {
            return ReadHelper.Instance.Distinct(binder, filter, modelSort);
        }

        #endregion
    }
}
