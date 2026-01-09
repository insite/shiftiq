using System;

namespace InSite.Persistence
{
    public static class CurrentPartition
    {
        public static bool IsE03 { get; set; }
        public static Guid OrganizationId { get; set; }
    }
}
