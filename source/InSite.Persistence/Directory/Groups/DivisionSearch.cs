using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class DivisionSearch
    {
        private class ReadHelper : ReadHelper<Division>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Division>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Divisions.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static Division Select(Guid identifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.Divisions.FirstOrDefault(x => x.DivisionIdentifier == identifier);
            }
        }

        #region Select (DivisionComboBox)

        public static ListItem[] SelectForDivisionComboBox(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.Divisions
                    .Where(x => x.OrganizationIdentifier == organization)
                    .OrderBy(x => x.DivisionCode ?? x.DivisionName)
                    .Select(x => new ListItem
                    {
                        Value = x.DivisionIdentifier.ToString(),
                        Text = x.DivisionCode ?? x.DivisionName
                    })
                    .ToArray();
            }
        }

        #endregion

        #region Select (filter)

        public static T[] BindByFilter<T>(Expression<Func<Division, T>> binder, DivisionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return FilterQuery(db.Divisions.AsQueryable(), filter)
                    .Select(binder)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public static int Count(DivisionFilter filter) =>
            ReadHelper.Instance.Count((IQueryable<Division> query) => FilterQuery(query, filter));

        private static IQueryable<Division> FilterQuery(IQueryable<Division> query, DivisionFilter filter)
        {
            if (filter.DivisionIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.DivisionIdentifiers.Contains(x.DivisionIdentifier));

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.DivisionName))
                query = query.Where(x => x.DivisionName.Contains(filter.DivisionName));

            if (!string.IsNullOrEmpty(filter.DivisionCode))
                query = query.Where(x => x.DivisionCode == filter.DivisionCode);

            if (filter.Created != null)
            {
                if (filter.Created.Since.HasValue)
                    query = query.Where(x => x.GroupCreated >= filter.Created.Since.Value);

                if (filter.Created.Before.HasValue)
                    query = query.Where(x => x.GroupCreated < filter.Created.Before.Value);
            }

            if (filter.CompanyName.HasValue())
                query = query.Where(x => x.Organization.CompanyName.Contains(filter.CompanyName));

            return query;
        }

        #endregion
    }
}
