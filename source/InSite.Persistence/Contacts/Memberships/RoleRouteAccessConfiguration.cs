using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class RoleRouteAccessConfiguration : EntityTypeConfiguration<RoleRouteAccess>
    {
        public RoleRouteAccessConfiguration() : this("security") { }

        public RoleRouteAccessConfiguration(string schema)
        {
            ToTable(schema + ".VRoleRouteAccess");
            HasKey(x => x.RouteUrl);
        }
    }
}
