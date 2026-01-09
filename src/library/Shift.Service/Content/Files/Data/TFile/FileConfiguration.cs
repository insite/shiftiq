using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class FileConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder) 
    {
        builder.ToTable("TFile", "assets");
        builder.HasKey(x => new { x.FileIdentifier });
            
            builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
            builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
            builder.Property(x => x.ObjectType).HasColumnName("ObjectType").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.ObjectIdentifier).HasColumnName("ObjectIdentifier").IsRequired();
            builder.Property(x => x.FileIdentifier).HasColumnName("FileIdentifier").IsRequired();
            builder.Property(x => x.FileName).HasColumnName("FileName").IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.FileSize).HasColumnName("FileSize").IsRequired();
            builder.Property(x => x.FileLocation).HasColumnName("FileLocation").IsRequired().IsUnicode(false).HasMaxLength(8);
            builder.Property(x => x.FileUrl).HasColumnName("FileUrl").IsUnicode(false).HasMaxLength(500);
            builder.Property(x => x.FilePath).HasColumnName("FilePath").IsUnicode(false).HasMaxLength(260);
            builder.Property(x => x.FileContentType).HasColumnName("FileContentType").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.FileUploaded).HasColumnName("FileUploaded").IsRequired();
            builder.Property(x => x.DocumentName).HasColumnName("DocumentName").IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.FileDescription).HasColumnName("FileDescription").IsUnicode(false).HasMaxLength(2400);
            builder.Property(x => x.FileCategory).HasColumnName("FileCategory").IsUnicode(false).HasMaxLength(120);
            builder.Property(x => x.FileSubcategory).HasColumnName("FileSubcategory").IsUnicode(false).HasMaxLength(120);
            builder.Property(x => x.FileStatus).HasColumnName("FileStatus").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.FileExpiry).HasColumnName("FileExpiry");
            builder.Property(x => x.FileReceived).HasColumnName("FileReceived");
            builder.Property(x => x.FileAlternated).HasColumnName("FileAlternated");
            builder.Property(x => x.LastActivityTime).HasColumnName("LastActivityTime");
            builder.Property(x => x.LastActivityUserIdentifier).HasColumnName("LastActivityUserIdentifier");
            builder.Property(x => x.ReviewedTime).HasColumnName("ReviewedTime");
            builder.Property(x => x.ReviewedUserIdentifier).HasColumnName("ReviewedUserIdentifier");
            builder.Property(x => x.ApprovedTime).HasColumnName("ApprovedTime");
            builder.Property(x => x.ApprovedUserIdentifier).HasColumnName("ApprovedUserIdentifier");

    }
}