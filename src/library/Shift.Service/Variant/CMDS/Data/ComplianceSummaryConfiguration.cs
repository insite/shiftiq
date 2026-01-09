using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryConfiguration : IEntityTypeConfiguration<ComplianceSummaryEntity>
{
    public void Configure(EntityTypeBuilder<ComplianceSummaryEntity> builder)
    {
        builder.HasKey(x => new { x.SummaryIdentifier });
    }
}
