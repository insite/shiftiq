using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TCourseConfiguration : EntityTypeConfiguration<TCourse>
    {
        public TCourseConfiguration() : this("courses") { }

        public TCourseConfiguration(string schema)
        {
            ToTable(schema + ".TCourse");
            HasKey(x => new { x.CourseIdentifier });

            Property(x => x.CourseAsset).IsRequired();
            Property(x => x.CourseCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.CourseDescription).IsOptional().IsUnicode(false);
            Property(x => x.CourseHook).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseIcon).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.CourseImage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseIsHidden).IsRequired();
            Property(x => x.CourseLabel).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.CourseLevel).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CourseProgram).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.CourseSequence).IsOptional();
            Property(x => x.CourseSlug).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CourseStyle).IsOptional().IsUnicode(false);
            Property(x => x.CourseUnit).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.FrameworkIdentifier).IsOptional();
            Property(x => x.GradebookIdentifier).IsOptional();
            Property(x => x.IsMultipleUnitsEnabled).IsRequired();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.SourceIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.CourseFlagColor).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.CourseFlagText).IsOptional().IsUnicode(false).HasMaxLength(50);

            HasOptional(a => a.Catalog).WithMany(b => b.Courses).HasForeignKey(a => a.CatalogIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Gradebook).WithMany(b => b.Courses).HasForeignKey(a => a.GradebookIdentifier).WillCascadeOnDelete(false);
        }
    }
}