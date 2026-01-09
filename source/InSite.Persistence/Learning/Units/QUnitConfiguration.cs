using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QUnitConfiguration : EntityTypeConfiguration<QUnit>
    {
        public QUnitConfiguration() : this("courses") { }

        public QUnitConfiguration(string schema)
        {
            ToTable(schema + ".QUnit");
            HasKey(x => x.UnitIdentifier);

            Property(x => x.UnitCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.UnitName).IsUnicode(false).HasMaxLength(200);

            HasRequired(a => a.Course).WithMany(b => b.Units).HasForeignKey(a => a.CourseIdentifier).WillCascadeOnDelete(false);
        }
    }
}
