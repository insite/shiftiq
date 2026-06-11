using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QActivityConfiguration : EntityTypeConfiguration<QActivity>
    {
        public QActivityConfiguration() : this("courses") { }

        public QActivityConfiguration(string schema)
        {
            ToTable(schema + ".QActivity");
            HasKey(x => new { x.ActivityIdentifier });

            Property(x => x.ActivityAuthorName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ActivityCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.ActivityHook).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ActivityImage).IsUnicode(false).HasMaxLength(200);
            Property(x => x.ActivityName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ActivityType).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ActivityUrl).IsUnicode(false).HasMaxLength(500);
            Property(x => x.ActivityUrlTarget).IsUnicode(false).HasMaxLength(500);
            Property(x => x.ActivityUrlType).IsUnicode(false).HasMaxLength(500);
            Property(x => x.PrerequisiteDeterminer).IsUnicode(false).HasMaxLength(3);
            Property(x => x.RequirementCondition).IsUnicode(false).HasMaxLength(30);
            Property(x => x.DoneButtonText).IsUnicode(false).HasMaxLength(24);

            HasOptional(a => a.GradeItem).WithMany(b => b.Activities).HasForeignKey(a => a.GradeItemIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Module).WithMany(b => b.Activities).HasForeignKey(a => a.ModuleIdentifier).WillCascadeOnDelete(false);
        }
    }
}
