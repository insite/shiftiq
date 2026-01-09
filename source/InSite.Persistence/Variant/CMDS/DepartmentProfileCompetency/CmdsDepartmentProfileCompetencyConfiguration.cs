using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsDepartmentProfileCompetencyConfiguration : EntityTypeConfiguration<CmdsDepartmentProfileCompetency>
    {
        public CmdsDepartmentProfileCompetencyConfiguration() : this("custom_cmds") { }

        public CmdsDepartmentProfileCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".DepartmentProfileCompetency");
            HasKey(x => new { x.CompetencyStandardIdentifier, x.DepartmentIdentifier, x.ProfileStandardIdentifier });
        
            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.Criticality).IsRequired().IsUnicode(false).HasMaxLength(12);
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.ValidForCount).IsOptional();
            Property(x => x.ValidForUnit).IsOptional().IsUnicode(false).HasMaxLength(6);
        }
    }
}
