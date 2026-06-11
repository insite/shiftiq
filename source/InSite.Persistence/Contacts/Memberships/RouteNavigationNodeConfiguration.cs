using System.Data.Entity.ModelConfiguration;

using Shift.Common;

namespace InSite.Persistence
{
    public class RouteNavigationNodeConfiguration : EntityTypeConfiguration<RouteNavigationNode>
    {
        public RouteNavigationNodeConfiguration() : this("setup") { }

        public RouteNavigationNodeConfiguration(string schema)
        {
            ToTable(schema + ".VRouteNavigationNode");
            HasKey(x => x.RouteUrl);
        }
    }
}
