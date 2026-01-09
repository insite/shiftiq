using System;

using Shift.Common;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Provides functions to convert between instances of IChange and SerializedChange.
    /// </summary>
    public static class ChangeExtensions
    {
        /// <summary>
        /// Returns a deserialized change.
        /// </summary>
        public static IChange Deserialize(this SerializedChange x)
        {
            var type = Registries.TypeRegistry.GetChangeType(x.ChangeType)
                ?? throw new ChangeNotFoundException(x.ChangeType);

            var serializer = Services.ServiceLocator.Instance.GetService<IJsonSerializer>();
            var data = serializer.Deserialize<IChange>(x.ChangeData, type, false);

            CopyChangeProperties(x, data);

            return data;
        }

        /// <summary>
        /// Returns a deserialized change.
        /// </summary>
        public static T Deserialize<T>(this SerializedChange x) where T : IChange
        {
            var serializer = Services.ServiceLocator.Instance.GetService<IJsonSerializer>();
            var data = serializer.Deserialize<T>(x.ChangeData, typeof(T), false);

            CopyChangeProperties(x, data);

            return data;
        }

        private static void CopyChangeProperties(SerializedChange source, IChange dest)
        {
            dest.AggregateIdentifier = source.AggregateIdentifier;
            dest.AggregateVersion = source.AggregateVersion;
            dest.ChangeTime = source.ChangeTime;
            dest.OriginOrganization = source.OriginOrganization;
            dest.OriginUser = source.OriginUser;
        }

        /// <summary>
        /// Returns a serialized change.
        /// </summary>
        public static SerializedChange Serialize(this IChange change, Guid aggregateIdentifier, int version)
        {
            var serializer = Services.ServiceLocator.Instance.GetService<IJsonSerializer>();
            var data = serializer.SerializeChange(change);

            var serialized = new SerializedChange
            {
                AggregateIdentifier = aggregateIdentifier,
                AggregateVersion = version,

                ChangeTime = change.ChangeTime,
                ChangeType = change.GetType().Name,
                ChangeData = data,

                OriginOrganization = change.OriginOrganization,
                OriginUser = change.OriginUser
            };

            change.OriginOrganization = serialized.OriginOrganization;
            change.OriginUser = serialized.OriginUser;

            return serialized;
        }
    }
}