using System;

namespace InSite
{
    public class StartupRequirements
    {
        public bool RequireDatabaseConnection { get; set; } = true;

        public bool RequirePartitionSettings { get; set; } = true;

        public Guid? DefaultOrganizationId { get; set; } = null;
    }
}