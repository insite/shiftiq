using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Json;

namespace InSite.Persistence
{
    public static class OrganizationAdapter
    {
        private static IJsonSerializer _serializer = new Serializer();

        public static OrganizationState CreatePacket(VOrganization entity)
        {
            if (entity == null)
                return null;
            var t = _serializer.Deserialize<OrganizationState>(entity.OrganizationData);
            t.OrganizationIdentifier = entity.OrganizationIdentifier;
            return t;
        }
    }
}