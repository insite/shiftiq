using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class DuplicateEmailConfiguration : EntityTypeConfiguration<DuplicateEmail>
    {
        public DuplicateEmailConfiguration() : this("identities") { }

        public DuplicateEmailConfiguration(string schema)
        {
            ToTable(schema + ".DuplicateEmail");
            HasKey(x => new { x.OrganizationIdentifier, x.UserEmail });
        
            Property(x => x.DuplicateCount).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserIdentifiers).IsOptional().IsUnicode(true);
        }
    }
}
