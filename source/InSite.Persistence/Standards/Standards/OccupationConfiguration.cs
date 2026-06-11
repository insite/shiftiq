using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class OccupationConfiguration : EntityTypeConfiguration<Occupation>
    {
        public OccupationConfiguration() : this("standards") { }

        public OccupationConfiguration(string schema)
        {
            ToTable(schema + ".Occupation");
            HasKey(x => new { x.OccupationKey });
        }
    }
}
