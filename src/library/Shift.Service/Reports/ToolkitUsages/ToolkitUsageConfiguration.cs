using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Reports;

internal class ToolkitUsageConfiguration : IEntityTypeConfiguration<ToolkitUsageEntity>
{
    public void Configure(EntityTypeBuilder<ToolkitUsageEntity> builder) 
    {
        builder.ToTable("TToolkitUsage", "reports");
        builder.HasKey(x => new { x.ToolkitUsageIdentifier });
    }
}
