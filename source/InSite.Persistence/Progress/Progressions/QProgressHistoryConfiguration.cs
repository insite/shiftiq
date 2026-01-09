using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QProgressHistoryConfiguration : EntityTypeConfiguration<QProgressHistory>
    {
        public QProgressHistoryConfiguration() : this("records") { }

        public QProgressHistoryConfiguration(string schema)
        {
            ToTable(schema + ".QProgressHistory");
            HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

            Property(x => x.AggregateIdentifier).IsRequired();
            Property(x => x.AggregateVersion).IsRequired();
            Property(x => x.ChangeBy).IsRequired();
            Property(x => x.ChangeTime).IsRequired();
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradeItemIdentifier).IsRequired();
            Property(x => x.ProgressTime).IsOptional();
            Property(x => x.ProgressType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
