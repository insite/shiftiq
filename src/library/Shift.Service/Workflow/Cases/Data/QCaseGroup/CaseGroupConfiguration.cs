using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workflow;

public class CaseGroupConfiguration : IEntityTypeConfiguration<CaseGroupEntity>
{
    public void Configure(EntityTypeBuilder<CaseGroupEntity> builder)
    {
        builder.ToTable("QCaseGroup", "issues");
        builder.HasKey(x => new { x.JoinIdentifier });

        builder.Property(x => x.CaseIdentifier).HasColumnName("CaseIdentifier").IsRequired();
        builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier").IsRequired();
        builder.Property(x => x.CaseRole).HasColumnName("CaseRole").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");
        builder.Property(x => x.JoinIdentifier).HasColumnName("JoinIdentifier").IsRequired();

    }
}