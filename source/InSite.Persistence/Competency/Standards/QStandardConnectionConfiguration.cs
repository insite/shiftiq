using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardConnectionConfiguration : EntityTypeConfiguration<QStandardConnection>
    {
        public QStandardConnectionConfiguration() : this("standard") { }

        public QStandardConnectionConfiguration(string schema)
        {
            ToTable("QStandardConnection", schema);
            HasKey(x => new { x.FromStandardIdentifier, x.ToStandardIdentifier });

            Property(x => x.ConnectionType).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
