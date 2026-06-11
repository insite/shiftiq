using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CompetencyCategoryConfiguration : EntityTypeConfiguration<CompetencyCategory>
    {
        public CompetencyCategoryConfiguration() : this("custom_cmds") { }

        public CompetencyCategoryConfiguration(string schema)
        {
            ToTable(schema + ".CompetencyCategory");
            HasKey(x => new { x.CompetencyStandardIdentifier });
        
            Property(x => x.CategoryName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.CompetencyNumber).IsUnicode(false).HasMaxLength(256);
            Property(x => x.CompetencyAchievements).IsOptional().IsUnicode(true).HasMaxLength(8000);
        }
    }
}
