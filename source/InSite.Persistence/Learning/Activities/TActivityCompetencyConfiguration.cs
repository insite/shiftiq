using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TActivityCompetencyConfiguration : EntityTypeConfiguration<TActivityCompetency>
    {
        public TActivityCompetencyConfiguration() : this("courses") { }

        public TActivityCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".TActivityCompetency");
            HasKey(x => new { x.ActivityIdentifier, x.CompetencyIdentifier });
            Property(x => x.ActivityIdentifier).IsRequired();
            Property(x => x.CompetencyIdentifier).IsRequired();
        }
    }
}
