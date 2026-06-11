using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QProgressConfiguration : EntityTypeConfiguration<QProgress>
    {
        public QProgressConfiguration() : this("records") { }

        public QProgressConfiguration(string schema)
        {
            ToTable(schema + ".QProgress");
            HasKey(x => new { x.ProgressIdentifier });

            HasRequired(a => a.Gradebook).WithMany(b => b.Progresses).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.GradeItem).WithMany(b => b.Progresses).HasForeignKey(c => c.GradeItemIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Learner).WithMany(b => b.Progresses).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);

            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradeItemIdentifier).IsRequired();
            Property(x => x.ProgressComment).IsOptional().IsUnicode(false).HasMaxLength(1200);
            Property(x => x.ProgressCompleted).IsOptional();
            Property(x => x.ProgressElapsedSeconds).IsOptional();
            Property(x => x.ProgressGraded).IsOptional();
            Property(x => x.ProgressIsCompleted).IsRequired();
            Property(x => x.ProgressIsDisabled).IsRequired();
            Property(x => x.ProgressIsLocked).IsRequired();
            Property(x => x.ProgressIsPublished).IsRequired();
            Property(x => x.ProgressMaxPoints).HasPrecision(28, 4).IsOptional();
            Property(x => x.ProgressNumber).HasPrecision(28, 4).IsOptional();
            Property(x => x.ProgressPassOrFail).IsOptional().IsUnicode(false).HasMaxLength(4);
            Property(x => x.ProgressPercent).HasPrecision(5, 4).IsOptional();
            Property(x => x.ProgressPoints).HasPrecision(28, 4).IsOptional();
            Property(x => x.ProgressStarted).IsOptional();
            Property(x => x.ProgressStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ProgressText).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
