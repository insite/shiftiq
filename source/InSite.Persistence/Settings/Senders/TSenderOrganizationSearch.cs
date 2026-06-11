using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public class TSenderOrganizationSearch
    {
        private class TSenderOrganizationReadHelper : ReadHelper<TSenderOrganization>
        {
            public static readonly TSenderOrganizationReadHelper Instance = new TSenderOrganizationReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TSenderOrganization>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TSenderOrganizations.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static bool Exists(Guid sender, Guid organization) =>
            TSenderOrganizationReadHelper.Instance.Exists(x => x.SenderIdentifier == sender && x.OrganizationIdentifier == organization);

        public static int Count(TSenderOrganizationFilter filter) =>
            TSenderOrganizationReadHelper.Instance.Count(
                (IQueryable<TSenderOrganization> query) => query.Filter(filter));

        public static IList<T> Bind<T>(
            Expression<Func<TSenderOrganization, T>> binder,
            TSenderOrganizationFilter filter,
            string modelSort = null, string entitySort = null)
        {
            return TSenderOrganizationReadHelper.Instance.Bind(
                (IQueryable<TSenderOrganization> query) => query.Select(binder),
                (IQueryable<TSenderOrganization> query) => query.Filter(filter),
                modelSort, entitySort);
        }
    }
}
