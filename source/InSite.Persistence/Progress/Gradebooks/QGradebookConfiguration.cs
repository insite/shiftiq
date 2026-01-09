using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QGradebookConfiguration : EntityTypeConfiguration<QGradebook>
    {
        public QGradebookConfiguration() : this("records") { }

        public QGradebookConfiguration(string schema)
        {
            ToTable(schema + ".QGradebook");
            HasKey(x => new { x.GradebookIdentifier });

            HasOptional(a => a.Event).WithMany(b => b.Gradebooks).HasForeignKey(c => c.EventIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Achievement).WithMany(b => b.Gradebooks).HasForeignKey(c => c.AchievementIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Framework).WithMany(b => b.Gradebooks).HasForeignKey(c => c.FrameworkIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Period).WithMany(b => b.Gradebooks).HasForeignKey(c => c.PeriodIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AchievementIdentifier).IsOptional();
            Property(x => x.EventIdentifier).IsOptional();
            Property(x => x.GradebookCreated).IsRequired();
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradebookTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.GradebookType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.IsLocked).IsRequired();
            Property(x => x.Reference).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FrameworkIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
