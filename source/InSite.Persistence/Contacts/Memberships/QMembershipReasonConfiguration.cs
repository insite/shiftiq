using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QMembershipReasonConfiguration : EntityTypeConfiguration<QMembershipReason>
    {
        public QMembershipReasonConfiguration() : this("contacts") { }

        public QMembershipReasonConfiguration(string schema)
        {
            ToTable("QMembershipReason", schema);
            HasKey(x => x.ReasonIdentifier);

            Property(x => x.ReasonType).IsUnicode(false).HasMaxLength(30).IsRequired();
            Property(x => x.ReasonSubtype).IsUnicode(false).HasMaxLength(30);
            Property(x => x.PersonOccupation).IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeType).IsUnicode(false).HasMaxLength(100).IsRequired();

            HasRequired(a => a.Membership).WithMany(b => b.Reasons).HasForeignKey(a => a.MembershipIdentifier).WillCascadeOnDelete(false);
        }
    }
}
