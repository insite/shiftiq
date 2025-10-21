using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCollectionItemConfiguration : EntityTypeConfiguration<TCollectionItem>
    {
        public TCollectionItemConfiguration() : this("utilities") { }

        public TCollectionItemConfiguration(string schema)
        {
            ToTable(schema + ".TCollectionItem");
            HasKey(x => x.ItemIdentifier);

            HasRequired(a => a.Collection).WithMany(b => b.Items).HasForeignKey(c => c.CollectionIdentifier).WillCascadeOnDelete(false);

            HasOptional(a => a.Organization).WithMany(b => b.CollectionItems).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);

            Property(x => x.GroupIdentifier).IsOptional();
            Property(x => x.ItemColor).IsOptional().IsUnicode(false).HasMaxLength(7);
            Property(x => x.ItemDescription).IsOptional().IsUnicode(false).HasMaxLength(800);
            Property(x => x.ItemFolder).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.ItemHours).IsOptional();
            Property(x => x.ItemIcon).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ItemIdentifier).IsRequired();
            Property(x => x.ItemIsDisabled).IsRequired();
            Property(x => x.ItemName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ItemNameTranslation).IsOptional().IsUnicode(false);
            Property(x => x.ItemNumber).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ItemSequence).IsRequired();
            Property(x => x.OrganizationIdentifier).IsOptional();
        }
    }
}