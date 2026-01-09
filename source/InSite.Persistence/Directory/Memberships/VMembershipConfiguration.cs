using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VMembershipConfiguration : EntityTypeConfiguration<VMembership>
    {
        public VMembershipConfiguration() : this("contacts") { }

        public VMembershipConfiguration(string schema)
        {
            ToTable(schema + ".VMembership");
            HasKey(x => new { x.GroupIdentifier, x.UserIdentifier });

            HasRequired(a => a.User).WithMany(b => b.Memberships).HasForeignKey(c => c.UserIdentifier);
            HasRequired(a => a.Group).WithMany(b => b.VMemberships).HasForeignKey(c => c.GroupIdentifier);
        }
    }
}
