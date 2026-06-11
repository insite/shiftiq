using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardOrganizationConfiguration : EntityTypeConfiguration<StandardOrganization>
    {
        public StandardOrganizationConfiguration() : this("standards") { }

        public StandardOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".StandardOrganization");

            HasKey(x => new { x.StandardIdentifier,x.OrganizationIdentifier });

            Property(x => x.StandardIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(a => a.Standard).WithMany(b => b.StandardOrganizations).HasForeignKey(a => a.StandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Organization).WithMany(b => b.StandardOrganizations).HasForeignKey(a => a.OrganizationIdentifier).WillCascadeOnDelete(false);
        }
    }
}
