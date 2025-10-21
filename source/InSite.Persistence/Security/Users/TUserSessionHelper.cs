using System;
using System.Data.Entity;
using System.Linq;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class TUserSessionHelper
    {
        internal static IQueryable<TUserSession> Filter(IQueryable<TUserSession> query, TUserSessionFilter filter, InternalDbContext db)
        {
            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.UserEmail))
                query = query.Where(x => x.UserEmail.Contains(DbFunctions.AsNonUnicode(filter.UserEmail)));

            if (!string.IsNullOrEmpty(filter.UserHostAddress))
                query = query.Where(x => x.UserHostAddress.Contains(DbFunctions.AsNonUnicode(filter.UserHostAddress)));

            if (!string.IsNullOrEmpty(filter.UserBrowser))
                query = query.Where(x => x.UserBrowser.Contains(DbFunctions.AsNonUnicode(filter.UserBrowser)));

            if (!string.IsNullOrEmpty(filter.UserAgent))
                query = query.Where(x => x.UserAgent.Contains(DbFunctions.AsNonUnicode(filter.UserAgent)));

            if (!string.IsNullOrEmpty(filter.UserLanguage))
                query = query.Where(x => x.UserLanguage.Contains(DbFunctions.AsNonUnicode(filter.UserLanguage)));

            if (filter.SessionIsAuthenticated.HasValue)
                query = query.Where(x => x.SessionIsAuthenticated == filter.SessionIsAuthenticated.Value);

            if (filter.SessionStartedSince.HasValue)
                query = query.Where(x => filter.SessionStartedSince.Value <= x.SessionStarted);

            if (filter.SessionStartedBefore.HasValue)
                query = query.Where(x => x.SessionStarted < filter.SessionStartedBefore.Value);

            query = ApplyOrganizationPersonTypesFilter(query, filter, db);

            return query;
        }

        private static IQueryable<TUserSession> ApplyOrganizationPersonTypesFilter(IQueryable<TUserSession> query, TUserSessionFilter filter, InternalDbContext db)
        {
            if (filter.OrganizationPersonTypes.IsNotEmpty())
            {
                var isAdministrator = filter.OrganizationPersonTypes.Contains(OrganizationPersonTypes.Administrator);
                var isLearner = filter.OrganizationPersonTypes.Contains(OrganizationPersonTypes.Learner);

                if (isAdministrator && isLearner)
                    query = query.Where(x =>
                        db.Persons.Any(
                            y => y.UserIdentifier == x.UserIdentifier
                                && y.OrganizationIdentifier == x.OrganizationIdentifier
                                && (y.IsAdministrator & y.IsLearner)
                        )
                    );

                else if (isAdministrator)
                    query = query.Where(x =>
                        db.Persons.Any(
                            y => y.UserIdentifier == x.UserIdentifier
                                && y.OrganizationIdentifier == x.OrganizationIdentifier
                                && y.IsAdministrator
                        )
                    );

                else if (isLearner)
                    query = query.Where(x =>
                        db.Persons.Any(
                            y => y.UserIdentifier == x.UserIdentifier
                                && y.OrganizationIdentifier == x.OrganizationIdentifier
                                && y.IsLearner
                        )
                    );
            }

            return query;
        }
    }
}
