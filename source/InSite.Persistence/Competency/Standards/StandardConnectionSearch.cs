using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public static class StandardConnectionSearch
    {
        private class ReadHelper : ReadHelper<StandardConnection>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<StandardConnection>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.StandardConnections.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public class ConnectionTreeInfo
        {
            public Guid FromStandardIdentifier { get; set; }
            public Guid ToStandardIdentifier { get; set; }
            public string ConnectionType { get; set; }
        }

        public static IReadOnlyList<StandardConnection> Select(
            Expression<Func<StandardConnection, bool>> filter,
            params Expression<Func<StandardConnection, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, includes);

        public static StandardConnection SelectFirst(
            Expression<Func<StandardConnection, bool>> filter,
            params Expression<Func<StandardConnection, object>>[] includes) =>
            ReadHelper.Instance.SelectFirst(filter, includes);

        public static IReadOnlyList<StandardConnection> Select(
            Expression<Func<StandardConnection, bool>> filter,
            string sortExpression,
            params Expression<Func<StandardConnection, object>>[] includes) =>
            ReadHelper.Instance.Select(filter, sortExpression, includes);

        public static T BindFirst<T>(
            Expression<Func<StandardConnection, T>> binder,
            Expression<Func<StandardConnection, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<StandardConnection, T>> binder,
            Expression<Func<StandardConnection, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static int Count(Expression<Func<StandardConnection, bool>> filter) =>
            ReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<StandardConnection, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        public static ConnectionTreeInfo[] SelectDownstream(IEnumerable<Guid> standardKeys) =>
            SelectDownstream<ConnectionTreeInfo>(standardKeys);

        public static T[] SelectDownstream<T>(IEnumerable<Guid> standardKeys)
        {
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>("EXEC standards.SelectStandardConnectionDownstream @StandardFilter", SqlParameterHelper.IdentifierList("@StandardFilter", standardKeys)).ToArray();
        }

        public static ConnectionTreeInfo[] SelectUpstream(IEnumerable<Guid> standardKeys) =>
            SelectUpstream<ConnectionTreeInfo>(standardKeys);

        public static T[] SelectUpstream<T>(IEnumerable<Guid> standardKeys)
        {
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>("EXEC standards.SelectStandardConnectionUpstream @StandardFilter", SqlParameterHelper.IdentifierList("@StandardFilter", standardKeys)).ToArray();
        }
    }
}
