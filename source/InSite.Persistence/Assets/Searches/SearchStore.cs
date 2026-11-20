using System;
using System.Data.Entity;

using InSite.Application.Assets.Read;

using Shift.Contract;

namespace InSite.Persistence
{
    public class SearchStore : ISearchStore
    {
        public void CreateSearch(bool isCmds)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 5 * 60;
                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "exec assets.CreateSearch @IsCmds = @p0", isCmds);
            }
        }

        public void EnqueueSearchObject(Guid? organizationId, Guid objectId, SearchObjectType objectType)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(@"
                    exec assets.EnqueueSearchObject
                        @OrganizationIdentifier = @p0
                        ,@ObjectIdentifier = @p1
                        ,@ObjectType = @p2
                    ",
                    organizationId,
                    objectId,
                    objectType.ToString()
                );
            }
        }

        public void EnqueueSearchPermission(Guid actionId, Guid groupId, SearchQueueOperation operation)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(@"
                    exec assets.EnqueueSearchPermission
                        @ActionIdentifier = @p0
                        ,@GroupIdentifier = @p1
                        ,@Operation = @p2
                    ",
                    actionId,
                    groupId,
                    operation.ToString()
                );
            }
        }

        public void EnqueueSearchPermission(Guid groupId, SearchQueueOperation operation)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(@"
                    exec assets.EnqueueSearchPermissionByGroup
                        @GroupIdentifier = @p0
                        ,@Operation = @p1
                    ",
                    groupId,
                    operation.ToString()
                );
            }
        }
    }
}
