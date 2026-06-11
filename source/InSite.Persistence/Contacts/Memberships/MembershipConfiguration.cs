using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class MembershipConfiguration : EntityTypeConfiguration<Membership>
    {
        public MembershipConfiguration() : this("contacts") { }

        public MembershipConfiguration(string schema)
        {
            ToTable("Membership", schema);
            HasKey(x => new { x.GroupIdentifier, x.UserIdentifier });

            Property(x => x.Assigned).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.MembershipType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.OrganizationIdentifier).IsOptional();
            Property(x => x.UserIdentifier).IsRequired();

            HasRequired(a => a.Group).WithMany(b => b.Memberships).HasForeignKey(a => a.GroupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.Memberships).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
