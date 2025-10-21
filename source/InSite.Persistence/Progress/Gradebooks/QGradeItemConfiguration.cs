using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QGradeItemConfiguration : EntityTypeConfiguration<QGradeItem>
    {
        public QGradeItemConfiguration() : this("records") { }

        public QGradeItemConfiguration(string schema)
        {
            ToTable(schema + ".QGradeItem");
            HasKey(x => x.GradeItemIdentifier);

            HasRequired(a => a.Gradebook).WithMany(b => b.Items).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Achievement).WithMany(b => b.GradeItems).HasForeignKey(c => c.AchievementIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Parent).WithMany(b => b.Children).HasForeignKey(c => c.ParentGradeItemIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AchievementElseCommand).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementFixedDate).IsOptional();
            Property(x => x.AchievementIdentifier).IsOptional();
            Property(x => x.AchievementThenCommand).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenChange).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AchievementWhenGrade).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradeItemCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GradeItemFormat).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.GradeItemHook).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GradeItemIdentifier).IsRequired();
            Property(x => x.GradeItemIsReported).IsRequired();
            Property(x => x.GradeItemMaxPoints).IsOptional();
            Property(x => x.GradeItemName).IsRequired().IsUnicode(false).HasMaxLength(400);
            Property(x => x.GradeItemPassPercent).IsOptional();
            Property(x => x.GradeItemReference).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GradeItemSequence).IsRequired();
            Property(x => x.GradeItemShortName).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.GradeItemType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.GradeItemWeighting).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ParentGradeItemIdentifier).IsOptional();
        }
    }
}
