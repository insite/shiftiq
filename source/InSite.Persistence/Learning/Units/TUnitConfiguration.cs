using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TUnitConfiguration : EntityTypeConfiguration<TUnit>
    {
        public TUnitConfiguration() : this("courses") { }

        public TUnitConfiguration(string schema)
        {
            ToTable(schema + ".TUnit");
            HasKey(x => new { x.UnitIdentifier });

            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.SourceIdentifier).IsOptional();
            Property(x => x.UnitAsset).IsRequired();
            Property(x => x.UnitCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.UnitIdentifier).IsRequired();
            Property(x => x.UnitName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.UnitSequence).IsRequired();

            HasRequired(a => a.Course).WithMany(b => b.Units).HasForeignKey(a => a.CourseIdentifier).WillCascadeOnDelete(false);
        }
    }
}
