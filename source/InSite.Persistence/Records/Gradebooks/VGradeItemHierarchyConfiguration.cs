using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VGradeItemHierarchyConfiguration : EntityTypeConfiguration<VGradeItemHierarchy>
    {
        public VGradeItemHierarchyConfiguration() : this("records") { }

        public VGradeItemHierarchyConfiguration(string schema)
        {
            ToTable(schema + ".VGradeItemHierarchy");
            HasKey(x => x.GradeItemIdentifier);

            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.GradeItemFormat).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.GradeItemIdentifier).IsRequired();
            Property(x => x.GradeItemName).IsRequired().IsUnicode(false).HasMaxLength(400);
            Property(x => x.GradeItemType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ParentGradeItemIdentifier).IsOptional();
            Property(x => x.PathCode).IsRequired().IsUnicode(false);
            Property(x => x.PathDepth).IsRequired();
            Property(x => x.PathIndent).IsRequired().IsUnicode(false).HasMaxLength(6);
            Property(x => x.PathSequence).IsRequired().IsUnicode(false);
        }
    }
}
