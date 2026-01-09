using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Workspace;

public class PageConfiguration : IEntityTypeConfiguration<PageEntity>
{
    public void Configure(EntityTypeBuilder<PageEntity> builder)
    {
        builder.ToTable("QPage", "sites");
        builder.HasKey(x => new { x.PageIdentifier });

        builder.Property(x => x.PageIdentifier).HasColumnName("PageIdentifier").IsRequired();
        builder.Property(x => x.PageType).HasColumnName("PageType").IsRequired().IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.Title).HasColumnName("Title").IsRequired().IsUnicode(true).HasMaxLength(128);
        builder.Property(x => x.Sequence).HasColumnName("Sequence").IsRequired();
        builder.Property(x => x.AuthorDate).HasColumnName("AuthorDate");
        builder.Property(x => x.AuthorName).HasColumnName("AuthorName").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ContentControl).HasColumnName("ContentControl").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.NavigateUrl).HasColumnName("NavigateUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.IsHidden).HasColumnName("IsHidden").IsRequired();
        builder.Property(x => x.ContentLabels).HasColumnName("ContentLabels").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.PageIcon).HasColumnName("PageIcon").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.Hook).HasColumnName("Hook").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.IsNewTab).HasColumnName("IsNewTab").IsRequired();
        builder.Property(x => x.PageSlug).HasColumnName("PageSlug").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.SiteIdentifier).HasColumnName("SiteIdentifier");
        builder.Property(x => x.ParentPageIdentifier).HasColumnName("ParentPageIdentifier");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime");
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.IsAccessDenied).HasColumnName("IsAccessDenied").IsRequired();
        builder.Property(x => x.ObjectType).HasColumnName("ObjectType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier");
    }
}
