using System;

using Shift.Common;

namespace InSite.Persistence
{
    public static class SessionHelper
    {
        public static void StartSession(Guid organizationId, Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                var session = new TUserSessionCache
                {
                    CacheIdentifier = UniqueIdentifier.Create(),
                    SessionStarted = DateTimeOffset.UtcNow,
                    OrganizationIdentifier = organizationId,
                    UserIdentifier = userId
                };
                db.TUserSessionCaches.Add(session);
                db.SaveChanges();
            }
        }
    }
}
