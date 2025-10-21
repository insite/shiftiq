using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardTierConfiguration : EntityTypeConfiguration<QStandardTier>
    {
        public QStandardTierConfiguration() : this("standard") { }

        public QStandardTierConfiguration(string schema)
        {
            ToTable("QStandardTier", schema);
            HasKey(x => x.ItemStandardIdentifier);

            Property(x => x.TierName).IsUnicode(false).HasMaxLength(12);
        }
    }
}
