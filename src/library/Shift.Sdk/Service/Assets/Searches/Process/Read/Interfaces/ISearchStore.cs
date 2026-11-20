using System;

using Shift.Contract;

namespace InSite.Application.Assets.Read
{
    public interface ISearchStore
    {
        void CreateSearch(bool isCmds);
        void EnqueueSearchObject(Guid? organizationId, Guid objectId, SearchObjectType objectType);
        void EnqueueSearchPermission(Guid actionId, Guid groupId, SearchQueueOperation operation);
        void EnqueueSearchPermission(Guid groupId, SearchQueueOperation operation);
    }
}
