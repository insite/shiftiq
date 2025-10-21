using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TEntityConfiguration : EntityTypeConfiguration<TEntity>
    {
        public TEntityConfiguration()
        {
            ToTable("TEntity", "metadata");
            HasKey(x => new { x.EntityId });

            Property(x => x.EntityId).IsRequired();
            Property(x => x.CollectionSlug).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CollectionKey).IsRequired().IsUnicode(false).HasMaxLength(60);
            Property(x => x.ComponentPart).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ComponentType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ComponentName).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EntityName).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.StorageKey).IsRequired().IsUnicode(false).HasMaxLength(80);
            Property(x => x.StorageSchema).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.StorageStructure).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.StorageTable).IsRequired().IsUnicode(false).HasMaxLength(40);
        }
    }
}
