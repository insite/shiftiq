using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public class DirectAccessSearch : IDirectAccessSearch
    {
        #region Classes

        private class IndividualReadHelper : ReadHelper<Individual>
        {
            public static readonly IndividualReadHelper Instance = new IndividualReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Individual>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Individuals.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public int CountIndividuals()
        {
            using (var db = new InternalDbContext())
            {
                return db.Individuals.Count();
            }
        }

        #endregion

        #region SELECT

        public string GetUniqueEmail(string individualEmail, Guid individualIdentifier, string personEmail, Guid personUserIdentifier)
        {
            if (individualIdentifier == personUserIdentifier)
                return individualEmail;

            if (StringHelper.Equals(individualEmail, personEmail))
                return individualEmail;

            const int MaximumDuplicates = 999;
            var count = 1;
            var email = $"duplicate-001-{individualEmail}";
            bool unique;

            do
            {
                var duplicate = UserSearch.SelectByEmail(email);
                unique = StringHelper.Equals(email, personEmail) || duplicate == null || duplicate.UserIdentifier == personUserIdentifier;

                if (!unique)
                {
                    count++;
                    var padded = StringHelper.PadLeft(count.ToString(), "0", 3);
                    email = $"duplicate-{padded}-{individualEmail}";
                }
            }
            while (!unique && count <= MaximumDuplicates);

            if (!unique && count > MaximumDuplicates)
                throw new Exception($"Unable to get a unique email address for {individualEmail}");

            return email;
        }

        public static Individual SelectIndividual(int key, params Expression<Func<Individual, object>>[] includes) =>
            IndividualReadHelper.Instance.SelectFirst(x => x.IndividualKey == key, includes);

        #endregion

        #region SELECT (IndividualFilter)

        public static int CountByFilter(IndividualFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CountByFilter(db, filter);
            }
        }

        private static int CountByFilter(InternalDbContext context, IndividualFilter filter) =>
            CreateQueryByFilter(filter, context).Count();

        public static SearchResultList SelectByFilter(IndividualFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        x.IndividualKey,
                        x.FirstName,
                        x.LastName,
                        x.Email,
                        x.AddressCity,
                        x.Refreshed,
                        x.RefreshedBy
                    })
                    .OrderBy(x => x.IndividualKey)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<Individual> CreateQueryByFilter(IndividualFilter filter, InternalDbContext db) =>
            FilterQueryByFilter(filter, db.Individuals.AsQueryable());

        private static IQueryable<Individual> FilterQueryByFilter(IndividualFilter filter, IQueryable<Individual> query)
        {
            if (filter.IndividualKeys.IsNotEmpty())
            {
                query = query.Where(x => filter.IndividualKeys.Contains(x.IndividualKey));
            }

            if (filter.FirstName.IsNotEmpty())
            {
                query = query.Where(x => x.FirstName.Contains(filter.FirstName));
            }

            if (filter.LastName.IsNotEmpty())
            {
                query = query.Where(x => x.LastName.Contains(filter.LastName));
            }

            if (filter.Email.IsNotEmpty())
            {
                query = query.Where(x => x.Email.Contains(filter.Email));
            }

            if (filter.City.IsNotEmpty())
            {
                query = query.Where(x => x.AddressCity.Contains(filter.City));
            }

            if (filter.IsActive.HasValue)
            {
                if (filter.IsActive.Value)
                    query = query.Where(x => x.IsActive == "Y");
                else
                    query = query.Where(x => x.IsActive == "N");
            }

            return query;
        }

        #endregion
    }
}
