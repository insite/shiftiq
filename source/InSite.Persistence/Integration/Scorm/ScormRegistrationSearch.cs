using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ScormRegistrationSearch
    {
        public static int Count(ScormRegistrationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        public static SearchResultList Select(ScormRegistrationFilter filter)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "LaunchedLast DESC";

            using (var db = new InternalDbContext())
            {
                var list = CreateQuery(filter, db)
                    .OrderBy(orderBy)
                    .ApplyPaging(filter)
                    .ToSearchResult();

                return list;
            }
        }

        public static TScormRegistration Select(Guid registration)
        {
            using (var db = new InternalDbContext())
            {
                return db.TScormRegistrations
                    .AsNoTracking()
                    .Include(x => x.Activities)
                    .FirstOrDefault(x => x.ScormRegistrationIdentifier == registration);
            }
        }

        public static TScormRegistration Select(string scorm, Guid learner)
        {
            using (var db = new InternalDbContext())
            {
                return db.TScormRegistrations
                    .AsNoTracking()
                    .Include(x => x.Activities)
                    .SingleOrDefault(x => x.ScormPackageHook == scorm && x.LearnerIdentifier == learner);
            }
        }

        public static List<TScormRegistration> Select(IEnumerable<string> hooks, IEnumerable<xApiCondition> conditions, Guid? learner)
        {
            using (var db = new InternalDbContext())
            {
                var activityIdentifiers = conditions.Select(x => x.ActivityIdentifier)
                    .ToHashSet();

                var query = db.TScormRegistrationActivities
                    .AsNoTracking()
                    .Where(x => hooks.Contains(x.Registration.ScormPackageHook) || activityIdentifiers.Contains(x.ActivityIdentifier))
                    .Join(db.TScormRegistrations,
                        activity => activity.ScormRegistrationIdentifier,
                        reg => reg.ScormRegistrationIdentifier,
                        (activity, reg) => reg
                    );

                if (learner.HasValue)
                    query = query.Where(x => x.LearnerIdentifier == learner);

                return query.Include(x => x.Activities).ToList();
            }
        }

        public static List<TScormRegistration> SelectByOrganizationIdentifier(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.TScormRegistrations
                    .AsNoTracking()
                    .Include(x => x.Activities)
                    .Where(x => x.OrganizationIdentifier == organization);

                return query.OrderByDescending(x => x.ScormLaunchedLast).ToList();
            }
        }

        private static IQueryable<TScormRegistration> CreateQuery(ScormRegistrationFilter filter, InternalDbContext db)
        {
            return db.TScormRegistrations.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);
        }
    }
}