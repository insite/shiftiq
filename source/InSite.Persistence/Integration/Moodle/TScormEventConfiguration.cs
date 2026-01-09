using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Integration.Moodle
{
    internal class TScormEventConfiguration : EntityTypeConfiguration<TScormEvent>
    {
        public TScormEventConfiguration()
        {
            ToTable("TScormEvent", "integration");
            HasKey(x => new { x.EventIdentifier });

            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.EventData).IsRequired().IsUnicode(false).HasMaxLength(2147483647);
            Property(x => x.EventWhen).IsRequired();
        }
    }
}
