using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardContainmentConfiguration : EntityTypeConfiguration<QStandardContainment>
    {
        public QStandardContainmentConfiguration() : this("standard") { }

        public QStandardContainmentConfiguration(string schema)
        {
            ToTable("QStandardContainment", schema);
            HasKey(x => new { x.ParentStandardIdentifier, x.ChildStandardIdentifier });

            Property(x => x.CreditType).IsOptional().IsUnicode(false).HasMaxLength(10);
        }
    }
}
