using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QUserConnectionConfiguration : EntityTypeConfiguration<QUserConnection>
    {
        public QUserConnectionConfiguration()
        {
            ToTable("QUserConnection", "identities");
            HasKey(x => new { x.FromUserIdentifier, x.ToUserIdentifier });

            Property(x => x.FromUserIdentifier).IsRequired();
            Property(x => x.ToUserIdentifier).IsRequired();
            Property(x => x.IsManager).IsRequired();
            Property(x => x.IsSupervisor).IsRequired();
            Property(x => x.IsValidator).IsRequired();
            Property(x => x.Connected).IsRequired();

            HasRequired(a => a.ToUser).WithMany(b => b.ToConnections).HasForeignKey(a => a.ToUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.FromUser).WithMany(b => b.FromConnections).HasForeignKey(a => a.FromUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
