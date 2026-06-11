using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TUserSessionSearch
    {
        private class ReadHelper : ReadHelper<TUserSession>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TUserSession>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TUserSessions.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(Expression<Func<TUserSession, T>> binder, TUserSessionFilter filter)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TUserSessions.AsQueryable().AsNoTracking();

                    IQueryable<T> bind(IQueryable<TUserSession> q) => q.Select(binder);

                    IQueryable<TUserSession> filterQuery(IQueryable<TUserSession> q) => TUserSessionHelper.Filter(q, filter, context);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, filter.OrderBy, null, false);

                    return modelQuery.ToArray();
                }
            }
        }

        public static IReadOnlyList<TUserSession> Select(
            Expression<Func<TUserSession, bool>> filter,
            params Expression<Func<TUserSession, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, includes);

        public static TUserSession SelectFirst(
            Expression<Func<TUserSession, bool>> filter,
            params Expression<Func<TUserSession, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<TUserSession> Select(
            Expression<Func<TUserSession, bool>> filter,
            string sortExpression,
            params Expression<Func<TUserSession, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, sortExpression, includes);

        public static TUserSession SelectFirst(
            Expression<Func<TUserSession, bool>> filter,
            string sortExpression,
            Expression<Func<TUserSession, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static IList<T> Bind<T>(
            Expression<Func<TUserSession, T>> binder,
            TUserSessionFilter filter) => 
            ReadHelper.Instance.Bind(binder, filter);

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<TUserSession, T>> binder,
            Expression<Func<TUserSession, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<TUserSession, T>> binder,
            Expression<Func<TUserSession, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static int Count(TUserSessionFilter filter)
        {
            using (var db = new InternalDbContext())
                return TUserSessionHelper.Filter(db.TUserSessions.AsQueryable().AsNoTracking(), filter, db).Count();
        }

        public static int Count(Expression<Func<TUserSession, bool>> filter) =>
            ReadHelper.Instance.Count(filter);

        public static List<TUserSessionDetail> Select(TUserSessionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var query = TUserSessionHelper.Filter(db.TUserSessions.AsQueryable().AsNoTracking(), filter, db)
                    .GroupJoin(
                        db.Persons,
                        userSession => new { userSession.UserIdentifier, userSession.OrganizationIdentifier },
                        person => new { person.UserIdentifier, person.OrganizationIdentifier },
                        (userSession, personGroup) => new { UserSession = userSession, Persons = personGroup.DefaultIfEmpty() }
                    )
                    .SelectMany(
                        x => x.Persons.Select(p =>
                            new TUserSessionDetail
                            {
                                UserIdentifier = x.UserSession.UserIdentifier,
                                AuthenticationErrorMessage = x.UserSession.AuthenticationErrorMessage,
                                AuthenticationErrorType = x.UserSession.AuthenticationErrorType,
                                AuthenticationSource = x.UserSession.AuthenticationSource,
                                OrganizationIdentifier = x.UserSession.OrganizationIdentifier,
                                SessionCode = x.UserSession.SessionCode,
                                SessionIdentifier = x.UserSession.SessionIdentifier,
                                SessionIsAuthenticated = x.UserSession.SessionIsAuthenticated,
                                SessionMinutes = x.UserSession.SessionMinutes,
                                SessionStarted = x.UserSession.SessionStarted,
                                SessionStopped = x.UserSession.SessionStopped,
                                UserAgent = x.UserSession.UserAgent,
                                UserBrowser = x.UserSession.UserBrowser,
                                UserBrowserVersion = x.UserSession.UserBrowserVersion,
                                UserEmail = x.UserSession.UserEmail,
                                UserHostAddress = x.UserSession.UserHostAddress,
                                UserLanguage = x.UserSession.UserLanguage,
                                IsLearner = (bool?)p.IsLearner ?? false, 
                                IsAdministrator = (bool?)p.IsAdministrator ?? false 
                            })
                    );

                if (!string.IsNullOrEmpty(filter.OrderBy))
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy("SessionStarted DESC, UserEmail ASC");

                return query
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public static bool Exists(Expression<Func<TUserSession, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        public static HistoryPerMonthChartItem[] SelectHistoryPerMonth(Guid organization)
        {
            const string query = @"
SELECT
    YEAR(Attempted)  AS [Year]
  , MONTH(Attempted) AS [Month]
  , COUNT(*)         AS [Count]
FROM
    logs.TUserSession
WHERE
    OrganizationIdentifier = @OrganizationIdentifier
GROUP BY
    YEAR(Attempted)
  , MONTH(Attempted)
ORDER BY
    YEAR(Attempted) DESC
  , MONTH(Attempted) DESC;
";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@OrganizationIdentifier", organization),
            };

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<HistoryPerMonthChartItem>(query, parameters).ToArray();
        }
    }
}