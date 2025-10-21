using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QMembershipConfiguration : EntityTypeConfiguration<QMembership>
    {
        public QMembershipConfiguration() : this("contacts") { }

        public QMembershipConfiguration(string schema)
        {
            ToTable("QMembership", schema);
            HasKey(x => x.MembershipIdentifier);

            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.MembershipIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.MembershipFunction).IsUnicode(false).HasMaxLength(20);
            Property(x => x.MembershipEffective).IsRequired();

            HasRequired(a => a.User).WithMany(b => b.Memberships).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Group).WithMany(b => b.QMemberships).HasForeignKey(a => a.GroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
