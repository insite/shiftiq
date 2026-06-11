using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UserConnectionConfiguration : EntityTypeConfiguration<UserConnection>
    {
        public UserConnectionConfiguration() : this("identities") { }

        public UserConnectionConfiguration(string schema)
        {
            ToTable(schema + ".UserConnection");
            HasKey(x => new { x.FromUserIdentifier, x.ToUserIdentifier });

            Property(x => x.Connected).IsRequired();
            Property(x => x.FromUserIdentifier).IsRequired();
            Property(x => x.IsManager).IsRequired();
            Property(x => x.IsSupervisor).IsRequired();
            Property(x => x.IsValidator).IsRequired();
            Property(x => x.ToUserIdentifier).IsRequired();

            HasRequired(a => a.FromUser).WithMany(b => b.DownstreamConnections).HasForeignKey(a => a.FromUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.ToUser).WithMany(b => b.UpstreamConnections).HasForeignKey(a => a.ToUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
