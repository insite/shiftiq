using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Reports;

internal class ToolkitVisitConfiguration : IEntityTypeConfiguration<ToolkitVisitEntity>
{
    public void Configure(EntityTypeBuilder<ToolkitVisitEntity> builder) 
    {
        builder.ToTable("TToolkitVisit", "reports");
        builder.HasKey(x => new { x.ToolkitVisitIdentifier });
    }
}
