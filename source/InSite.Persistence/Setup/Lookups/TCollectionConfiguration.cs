using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCollectionConfiguration : EntityTypeConfiguration<TCollection>
    {
        public TCollectionConfiguration() : this("utilities") { }

        public TCollectionConfiguration(string schema)
        {
            ToTable(schema + ".TCollection");
            HasKey(x => x.CollectionIdentifier);

            Property(x => x.CollectionName).IsRequired().IsUnicode(false).HasMaxLength(250);
            Property(x => x.CollectionPackage).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CollectionProcess).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CollectionReferences).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CollectionTool).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CollectionType).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
