using System.Data.Entity.ModelConfiguration;

using Shift.Common;

namespace InSite.Persistence
{
    public class RouteEndpointConfiguration : EntityTypeConfiguration<RouteEndpoint>
    {
        public RouteEndpointConfiguration() : this("setup") { }

        public RouteEndpointConfiguration(string schema)
        {
            ToTable(schema + ".VRouteEndpoint");
            HasKey(x => x.RouteId);
        }
    }
}
