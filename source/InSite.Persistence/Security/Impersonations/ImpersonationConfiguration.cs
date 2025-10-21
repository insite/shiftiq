using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class ImpersonationConfiguration : EntityTypeConfiguration<Impersonation>
    {
        public ImpersonationConfiguration() : this("identities") { }

        public ImpersonationConfiguration(string schema)
        {
            ToTable(schema + ".Impersonation");
            HasKey(x => x.ImpersonationIdentifier);

            Property(x => x.ImpersonatedUserIdentifier).IsRequired();
            Property(x => x.ImpersonationStarted).IsRequired();
            Property(x => x.ImpersonationStopped).IsOptional();
            Property(x => x.ImpersonatorUserIdentifier).IsRequired();
        }
    }
}