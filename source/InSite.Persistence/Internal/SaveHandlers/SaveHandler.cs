using System;
using System.Data.Entity;

using InSite.Application;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    internal class SaveHandler : ISaveHandler
    {
        public void Before(ServiceIdentity identity, InternalDbContext context)
        {
            var entries = context.ChangeTracker.Entries();
            var userIdentifier = identity?.User ?? UserIdentifiers.Someone;

            foreach (var entry in entries)
            {
                if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) && entry.Entity is IHasTimestamp timestamp)
                    UpdateTimestamp(timestamp, userIdentifier, entry.State == EntityState.Added);
            }
        }

        private static void UpdateTimestamp(IHasTimestamp timestamp, Guid identity, bool isNew)
        {
            timestamp.Modified = DateTimeOffset.UtcNow;
            timestamp.ModifiedBy = identity;

            if (isNew)
            {
                timestamp.CreatedBy = identity;
                timestamp.Created = timestamp.Modified;
            }
        }
    }
}