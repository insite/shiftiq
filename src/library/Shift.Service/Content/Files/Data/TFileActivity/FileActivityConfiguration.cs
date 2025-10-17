using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class FileActivityConfiguration : IEntityTypeConfiguration<FileActivityEntity>
{
    public void Configure(EntityTypeBuilder<FileActivityEntity> builder)
    {
        builder.ToTable("TFileActivity", "assets");
        builder.HasKey(x => x.ActivityIdentifier);

        builder.Property(x => x.FileIdentifier).HasColumnName("FileIdentifier").IsRequired();
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.ActivityIdentifier).HasColumnName("ActivityIdentifier").IsRequired();
        builder.Property(x => x.ActivityTime).HasColumnName("ActivityTime").IsRequired();
        builder.Property(x => x.ActivityChanges).HasColumnName("ActivityChanges").IsRequired().IsUnicode(false);
    }
}