using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardGroupConfiguration : EntityTypeConfiguration<QStandardGroup>
    {
        public QStandardGroupConfiguration() : this("standard") { }

        public QStandardGroupConfiguration(string schema)
        {
            ToTable("QStandardGroup", schema);
            HasKey(x => new { x.StandardIdentifier, x.GroupIdentifier });
        }
    }
}
