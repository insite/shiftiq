using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class VActivityCompetencyConfiguration : EntityTypeConfiguration<VActivityCompetency>
    {
        public VActivityCompetencyConfiguration() : this("courses") { }

        public VActivityCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".VActivityCompetency");
            HasKey(x => new { x.ActivityIdentifier, x.CompetencyIdentifier });

            Property(x => x.ActivityIdentifier).IsRequired();
            Property(x => x.CompetencyAsset).IsOptional();
            Property(x => x.CompetencyCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CompetencyIdentifier).IsRequired();
            Property(x => x.CompetencyTitle).IsOptional().IsUnicode(true);
            Property(x => x.CompetencyType).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.RelationshipType).IsOptional().IsUnicode(false).HasMaxLength(10);
        }
    }
}
