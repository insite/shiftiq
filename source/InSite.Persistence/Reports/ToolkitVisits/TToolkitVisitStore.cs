using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TToolkitVisitStore
    {
        public static void Visit(Guid organizationId, Guid userId, Guid tokenId, Guid actionId)
        {
            var visited = DateTimeOffset.UtcNow;

            using (var db = new InternalDbContext())
            {
                var entity = new TToolkitVisit
                {
                    ToolkitVisitIdentifier = UniqueIdentifier.Create(),
                    OrganizationIdentifier = organizationId,
                    UserIdentifier = userId,
                    TokenIdentifier = tokenId,
                    ActionIdentifier = actionId,
                    Visited = visited
                };

                db.TToolkitVisits.Add(entity);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.InnerException == null || !ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key row"))
                        throw;
                }
            }
        }
    }
}
