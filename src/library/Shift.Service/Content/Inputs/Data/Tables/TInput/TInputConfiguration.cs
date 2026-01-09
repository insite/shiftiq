using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class TInputConfiguration : IEntityTypeConfiguration<TInputEntity>
{
    public void Configure(EntityTypeBuilder<TInputEntity> builder) 
    {
        builder.ToTable("TContent", "contents");
        builder.HasKey(x => new { x.ContentIdentifier });
            
            builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
            builder.Property(x => x.ContainerIdentifier).HasColumnName("ContainerIdentifier").IsRequired();
            builder.Property(x => x.ContentLabel).HasColumnName("ContentLabel").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.ContentLanguage).HasColumnName("ContentLanguage").IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.ContentSnip).HasColumnName("ContentSnip").IsRequired().IsUnicode(true).HasMaxLength(100);
            builder.Property(x => x.ContentText).HasColumnName("ContentText").IsUnicode(true);
            builder.Property(x => x.ContentHtml).HasColumnName("ContentHtml").IsUnicode(true);
            builder.Property(x => x.ContainerType).HasColumnName("ContainerType").IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.ContentSequence).HasColumnName("ContentSequence");
            builder.Property(x => x.ContentIdentifier).HasColumnName("ContentIdentifier").IsRequired();
            builder.Property(x => x.ReferenceFiles).HasColumnName("ReferenceFiles").IsUnicode(false);
            builder.Property(x => x.ReferenceCount).HasColumnName("ReferenceCount");

    }
}