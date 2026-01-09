using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QMembershipDeletionConfiguration : EntityTypeConfiguration<QMembershipDeletion>
    {
        public QMembershipDeletionConfiguration() : this("contacts") { }

        public QMembershipDeletionConfiguration(string schema)
        {
            ToTable("QMembershipDeletion", schema);
            HasKey(x => x.DeletionIdentifier);

            Property(x => x.DeletionIdentifier).IsRequired();
            Property(x => x.DeletionWhen).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.MembershipIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
