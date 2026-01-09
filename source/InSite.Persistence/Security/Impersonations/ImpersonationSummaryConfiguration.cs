using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class ImpersonationSummaryConfiguration : EntityTypeConfiguration<ImpersonationSummary>
    {
        public ImpersonationSummaryConfiguration() : this("identities") { }

        public ImpersonationSummaryConfiguration(string schema)
        {
            ToTable(schema + ".ImpersonationSummary");
            HasKey(x => new { x.ImpersonatorUserIdentifier, x.ImpersonationStarted });
        
            Property(x => x.ImpersonatedUserFullName).IsOptional().IsUnicode(false).HasMaxLength(145);
            Property(x => x.ImpersonatedUserIdentifier).IsRequired();
            Property(x => x.ImpersonationStarted).IsRequired();
            Property(x => x.ImpersonationStopped).IsOptional();
            Property(x => x.ImpersonatorAccessGrantedToCmds).IsRequired();
            Property(x => x.ImpersonatorUserFullName).IsOptional().IsUnicode(false).HasMaxLength(145);
            Property(x => x.ImpersonatorUserIdentifier).IsRequired();
        }
    }
}
