using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserCompetencyExpirationConfiguration : EntityTypeConfiguration<UserCompetencyExpiration>
    {
        public UserCompetencyExpirationConfiguration() : this("custom_cmds") { }

        public UserCompetencyExpirationConfiguration(string schema)
        {
            ToTable(schema + ".UserCompetencyExpiration");
            HasKey(x => new { x.CompetencyStandardIdentifier, x.UserIdentifier });
        
            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.ExpirationDate).IsOptional();
            Property(x => x.LifetimeInMonths).IsOptional();
            Property(x => x.Notified).IsOptional();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.ValidationDate).IsOptional();
            Property(x => x.ValidationStatus).IsOptional().IsUnicode(false).HasMaxLength(32);
        }
    }
}
