using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardCategoryConfiguration : EntityTypeConfiguration<QStandardCategory>
    {
        public QStandardCategoryConfiguration() : this("standard") { }

        public QStandardCategoryConfiguration(string schema)
        {
            ToTable("QStandardCategory", schema);
            HasKey(x => new { x.StandardIdentifier, x.CategoryIdentifier });
        }
    }
}
