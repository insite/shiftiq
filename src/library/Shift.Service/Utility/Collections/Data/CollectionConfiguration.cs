using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Utility;

internal class CollectionConfiguration : IEntityTypeConfiguration<CollectionEntity>
{
    public void Configure(EntityTypeBuilder<CollectionEntity> builder)
    {
        builder.ToTable("TCollection", "utilities");
        builder.HasKey(x => x.CollectionIdentifier);
        
        builder.Property(x => x.CollectionName).IsRequired().IsUnicode(false).HasMaxLength(250);
        builder.Property(x => x.CollectionPackage).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CollectionProcess).IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CollectionReferences).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CollectionTool).IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CollectionType).IsRequired().IsUnicode(false).HasMaxLength(20);
    }
}
