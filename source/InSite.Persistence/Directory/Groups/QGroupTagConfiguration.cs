using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class QGroupTagConfiguration : EntityTypeConfiguration<QGroupTag>
    {
        public QGroupTagConfiguration() : this("contacts") { }

        public QGroupTagConfiguration(string schema)
        {
            ToTable(schema + ".QGroupTag");
            HasKey(x => x.TagIdentifier);

            Property(x => x.TagIdentifier).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupTag).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}
