using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class UploadConfiguration : IEntityTypeConfiguration<UploadEntity>
{
    public void Configure(EntityTypeBuilder<UploadEntity> builder)
    {
        builder.ToTable("Upload", "resources");
        builder.HasKey(x => x.UploadIdentifier);

        builder.Property(x => x.ContainerIdentifier).IsRequired();
        builder.Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.ContentFingerprint).IsUnicode(false).HasMaxLength(24);
        builder.Property(x => x.ContentSize);
        builder.Property(x => x.ContentType).IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.Description).IsUnicode(false).HasMaxLength(300);
        builder.Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.NavigateUrl).IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.OrganizationIdentifier).IsRequired();
        builder.Property(x => x.Title).IsRequired().IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.Uploaded).IsRequired();
        builder.Property(x => x.Uploader).IsRequired();
        builder.Property(x => x.UploadIdentifier).IsRequired();
        builder.Property(x => x.UploadPrivacyScope).IsRequired().IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.UploadType).IsRequired().IsUnicode(false).HasMaxLength(16);
    }
}
