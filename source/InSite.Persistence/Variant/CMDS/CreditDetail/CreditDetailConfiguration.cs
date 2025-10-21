using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CreditDetailConfiguration : EntityTypeConfiguration<CreditDetail>
    {
        public CreditDetailConfiguration() : this("custom_cmds") { }

        public CreditDetailConfiguration(string schema)
        {
            ToTable(schema + ".CreditDetail");
            HasKey(x => new { x.ResourceIdentifier, x.UserIdentifier });
        
            Property(x => x.ResourceType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.UserEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.ValidationStatus).IsUnicode(false).HasMaxLength(32);
        }
    }
}
