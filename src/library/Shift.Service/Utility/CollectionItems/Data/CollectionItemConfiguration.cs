using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Utility;

internal class CollectionItemConfiguration : IEntityTypeConfiguration<CollectionItemEntity>
{
    public void Configure(EntityTypeBuilder<CollectionItemEntity> builder)
    {
        builder.ToTable("TCollectionItem", "utilities");
        builder.HasKey(x => x.ItemIdentifier);

        builder.Property(x => x.GroupIdentifier);
        builder.Property(x => x.ItemColor).IsUnicode(false).HasMaxLength(7);
        builder.Property(x => x.ItemDescription).IsUnicode(false).HasMaxLength(800);
        builder.Property(x => x.ItemFolder).IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.ItemHours);
        builder.Property(x => x.ItemIcon).IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.ItemIdentifier).IsRequired();
        builder.Property(x => x.ItemIsDisabled).IsRequired();
        builder.Property(x => x.ItemName).IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.ItemNameTranslation).IsUnicode(false);
        builder.Property(x => x.ItemNumber).IsRequired();
        builder.Property(x => x.ItemSequence).IsRequired();
        builder.Property(x => x.OrganizationIdentifier);
    }
}
