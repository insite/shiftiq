using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardValidationLogConfiguration : EntityTypeConfiguration<QStandardValidationLog>
    {
        public QStandardValidationLogConfiguration() : this("standard") { }

        public QStandardValidationLogConfiguration(string schema)
        {
            ToTable("QStandardValidationLog", schema);
            HasKey(x => new { x.LogIdentifier });

            Property(x => x.LogComment).IsUnicode(false);
            Property(x => x.LogStatus).IsUnicode(false).HasMaxLength(50);
        }
    }
}
