using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class FileClaimConfiguration : IEntityTypeConfiguration<FileClaimEntity>
{
    public void Configure(EntityTypeBuilder<FileClaimEntity> builder) 
    {
        builder.ToTable("TFileClaim", "assets");
        builder.HasKey(x => new { x.ClaimIdentifier });
            
            builder.Property(x => x.FileIdentifier).HasColumnName("FileIdentifier").IsRequired();
            builder.Property(x => x.ClaimIdentifier).HasColumnName("ClaimIdentifier").IsRequired();
            builder.Property(x => x.ObjectType).HasColumnName("ObjectType").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier").IsRequired();
            builder.Property(x => x.ClaimGranted).HasColumnName("ClaimGranted");

    }
}