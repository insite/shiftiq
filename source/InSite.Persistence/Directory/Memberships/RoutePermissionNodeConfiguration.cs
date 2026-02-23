using System.Data.Entity.ModelConfiguration;

using Shift.Common;

namespace InSite.Persistence
{
    public class RoutePermissionNodeConfiguration : EntityTypeConfiguration<RoutePermissionNode>
    {
        public RoutePermissionNodeConfiguration() : this("setup") { }

        public RoutePermissionNodeConfiguration(string schema)
        {
            ToTable(schema + ".VRoutePermissionNode");
            HasKey(x => x.RouteUrl);
        }
    }
}
