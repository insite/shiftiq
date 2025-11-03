using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workspace;

public class SiteConfiguration : IEntityTypeConfiguration<SiteEntity>
{
    public void Configure(EntityTypeBuilder<SiteEntity> builder)
    {
        builder.ToTable("QSite", "sites");
        builder.HasKey(x => new { x.SiteIdentifier });

        builder.Property(x => x.SiteIdentifier).HasColumnName("SiteIdentifier").IsRequired();
        builder.Property(x => x.SiteDomain).HasColumnName("SiteDomain").IsRequired().IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.SiteTitle).HasColumnName("SiteTitle").IsRequired().IsUnicode(true).HasMaxLength(128);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime");
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsUnicode(false).HasMaxLength(100);
    }
}