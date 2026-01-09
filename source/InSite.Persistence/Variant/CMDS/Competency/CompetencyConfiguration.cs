using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CompetencyConfiguration : EntityTypeConfiguration<Competency>
    {
        public CompetencyConfiguration() : this("custom_cmds") { }

        public CompetencyConfiguration(string schema)
        {
            ToTable(schema + ".Competency");
            HasKey(x => new { x.StandardIdentifier });

            Property(x => x.Number).IsUnicode(false).HasMaxLength(256);
            Property(x => x.NumberOld).IsUnicode(false);
        }
    }
}
