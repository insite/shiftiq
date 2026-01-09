using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TSenderSearch
    {
        private class TSenderReadHelper : ReadHelper<TSender>
        {
            public static readonly TSenderReadHelper Instance = new TSenderReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TSender>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TSenders.Include(x => x.Messages).AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TSender Select(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                return db.TSenders.FirstOrDefault(x => x.SenderIdentifier == id);
            }
        }

        public static TSender[] Select(string type, Guid? organization, bool? enabled)
        {
            using (var db = new InternalDbContext())
            {
                var senders = db.TSenders.AsNoTracking().AsQueryable();

                if (type != null)
                    senders = senders.Where(x => x.SenderType == type);

                if (organization != null)
                    senders = senders.Where(x => !x.Organizations.Any() || x.Organizations.Any(t => t.OrganizationIdentifier == organization.Value));

                if (enabled != null)
                    senders = senders.Where(x => x.SenderEnabled == enabled.Value);

                return senders.OrderBy(x => x.SenderNickname).ToArray();
            }
        }

        public static TSender[] SelectAll()
        {
            using (var db = new InternalDbContext())
                return db.TSenders.OrderBy(x => x.SenderNickname).ToArray();
        }

        public static int Count(TSenderFilter filter) =>
            TSenderReadHelper.Instance.Count(
                (IQueryable<TSender> query) => query.Filter(filter));

        public static T[] BindByFilter<T>(Expression<Func<TSender, T>> binder, TSenderFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return db.TSenders
                    .AsQueryable()
                    .Filter(filter)
                    .Select(binder)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }
    }
}
