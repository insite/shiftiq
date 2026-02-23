using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shift.Common;

namespace Shift.Service.Setup;

public class RouteEndpointConfiguration : IEntityTypeConfiguration<RouteEndpoint>
{
    public void Configure(EntityTypeBuilder<RouteEndpoint> builder)
    {
        builder.ToTable("VRouteEndpoint", "setup");
        builder.HasKey(x => x.RouteId);
    }
}