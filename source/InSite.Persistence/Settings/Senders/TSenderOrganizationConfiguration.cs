using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TSenderOrganizationConfiguration : EntityTypeConfiguration<TSenderOrganization>
    {
        public TSenderOrganizationConfiguration() : this("accounts") { }

        public TSenderOrganizationConfiguration(string schema)
        {
            ToTable(schema + ".TSenderOrganization");

            HasKey(x => new { x.JoinIdentifier });

            Property(x => x.SenderIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.JoinIdentifier).IsRequired();

            HasRequired(a => a.Sender).WithMany(b => b.Organizations).HasForeignKey(c => c.SenderIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Organization).WithMany(b => b.Senders).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);
        }
    }
}
