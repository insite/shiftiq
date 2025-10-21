using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TActivityConfiguration : EntityTypeConfiguration<TActivity>
    {
        public TActivityConfiguration() : this("courses") { }

        public TActivityConfiguration(string schema)
        {
            ToTable(schema + ".TActivity");
            HasKey(x => new { x.ActivityIdentifier });

            Property(x => x.ActivityAsset).IsRequired();
            Property(x => x.ActivityAuthorDate).IsOptional();
            Property(x => x.ActivityAuthorName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ActivityCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ActivityHook).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ActivityIdentifier).IsRequired();
            Property(x => x.ActivityImage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ActivityName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ActivitySequence).IsRequired();
            Property(x => x.ActivityType).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ActivityUrl).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ActivityUrlTarget).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.ActivityUrlType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.AssessmentFormIdentifier).IsOptional();
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.GradeItemIdentifier).IsOptional();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.ModuleIdentifier).IsRequired();
            Property(x => x.SourceIdentifier).IsOptional();
            Property(x => x.SurveyFormIdentifier).IsOptional();
            Property(x => x.InteractionIdentifier).IsOptional();
            Property(x => x.PrerequisiteDeterminer).IsOptional().IsUnicode(false).HasMaxLength(3);
            Property(x => x.RequirementCondition).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.DoneButtonText).IsOptional().IsUnicode(false).HasMaxLength(24);

            HasOptional(a => a.GradeItem).WithMany(b => b.Activities).HasForeignKey(a => a.GradeItemIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Module).WithMany(b => b.Activities).HasForeignKey(a => a.ModuleIdentifier).WillCascadeOnDelete(false);
        }
    }
}
