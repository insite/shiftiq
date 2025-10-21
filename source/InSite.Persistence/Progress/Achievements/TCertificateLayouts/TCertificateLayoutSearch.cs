using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public static class TCertificateLayoutSearch
    {
        private class ReadHelper : ReadHelper<TCertificateLayout>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCertificateLayout>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCertificateLayouts.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TCertificateLayout Select(Guid id) =>
            ReadHelper.Instance.SelectFirst(x => x.CertificateLayoutIdentifier == id, null);

        public static TCertificateLayout Select(string code) => 
            ReadHelper.Instance.SelectFirst(x => x.CertificateLayoutCode == code, null);

        public static TCertificateLayout Select(string code, Guid organization) =>
            ReadHelper.Instance.SelectFirst(x => x.OrganizationIdentifier == organization && x.CertificateLayoutCode == code, null);

        public static T[] Bind<T>(
            Expression<Func<TCertificateLayout, T>> binder,
            Expression<Func<TCertificateLayout, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static IList<T> Bind<T>(Expression<Func<TCertificateLayout, T>> binder, TCertificateLayoutFilter filter)
        {
            return ReadHelper.Instance.Bind(
                (IQueryable<TCertificateLayout> query) => query.Select(binder),
                (IQueryable<TCertificateLayout> query) => query.Filter(filter),
                filter.Paging, filter.OrderBy, null);
        }

        public static bool Exists(Expression<Func<TCertificateLayout, bool>> filter) =>
            ReadHelper.Instance.Exists(filter);

        public static bool Exists(Guid id) =>
            ReadHelper.Instance.Exists(x => x.CertificateLayoutIdentifier == id);

        public static int Count(Expression<Func<TCertificateLayout, bool>> filter) =>
            ReadHelper.Instance.Count(filter);

        public static int Count(TCertificateLayoutFilter filter) =>
            ReadHelper.Instance.Count(
                (IQueryable<TCertificateLayout> query) => query.Filter(filter));
    }
}
