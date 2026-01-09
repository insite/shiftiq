using System;

using Shift.Common;
namespace InSite.Persistence
{
    public partial class QCourseStore
    {
        enum EntityOperation { Insert, Modify, Remove, Ignore }

        interface IEntityState
        {
            EntityOperation Operation { get; }
            object EntityObject { get; }
            Guid OrganizationId { get; }
            Guid ContainerId { get; }
            string ContainerType { get; }
            ContentContainer Content { get; }
        }

        class EntityState<T> : IEntityState
        {
            public EntityOperation Operation { get; }
            public T Entity { get; }
            public Guid OrganizationId { get; }
            public Guid ContainerId { get; }
            public string ContainerType { get; }
            public ContentContainer Content { get; set; }

            public object EntityObject => Entity;

            public EntityState(EntityOperation operation, T entity)
            {
                Operation = operation;
                Entity = entity;
                OrganizationId = Guid.Empty;
                ContainerId = Guid.Empty;
                ContainerType = null;
                Content = null;
            }

            public EntityState(EntityOperation operation, T entity, Guid organizationId, Guid containerId, string containerType, ContentContainer content)
            {
                Operation = operation;
                Entity = entity;
                OrganizationId = organizationId;
                ContainerId = containerId;
                ContainerType = containerType;
                Content = content;
            }
        }
    }
}
