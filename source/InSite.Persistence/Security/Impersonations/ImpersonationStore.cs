using System;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class ImpersonationStore
    {
        public static void Start(Guid impersonated, Guid impersonator)
        {
            using (var db = new InternalDbContext())
            {
                var i = new Impersonation
                {
                    ImpersonationIdentifier = UniqueIdentifier.Create(),
                    ImpersonatedUserIdentifier = impersonated,
                    ImpersonatorUserIdentifier = impersonator,
                    ImpersonationStarted = DateTimeOffset.UtcNow
                };
                db.Impersonations.Add(i);
                db.SaveChanges();
            }
        }

        public static void Stop(Guid impersonator)
        {
            using (var db = new InternalDbContext())
            {
                var i = db.Impersonations
                    .Where(x => x.ImpersonatorUserIdentifier == impersonator && x.ImpersonationStopped == null)
                    .OrderByDescending(x => x.ImpersonationStarted)
                    .FirstOrDefault();

                if (i != null)
                {
                    i.ImpersonationStopped = DateTimeOffset.UtcNow;
                    db.SaveChanges();
                }
            }
        }
    }
}
