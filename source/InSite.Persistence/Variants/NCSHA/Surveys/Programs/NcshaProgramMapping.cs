using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class NcshaProgramMapping : EntityTypeConfiguration<NcshaProgram>
    {
        public NcshaProgramMapping() : this("custom_ncsha")
        {
        }

        public NcshaProgramMapping(string schema)
        {
            ToTable(schema + ".Program");
            HasKey(x => new { x.ProgramId, x.ProgramCode });
        }
    }
}