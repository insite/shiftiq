using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardConnectionConfiguration : EntityTypeConfiguration<StandardConnection>
    {
        public StandardConnectionConfiguration() : this("standards") { }

        public StandardConnectionConfiguration(string schema)
        {
            ToTable(schema + ".StandardConnection");
            HasKey(x => new { x.FromStandardIdentifier,x.ToStandardIdentifier });
            Property(x => x.ConnectionType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.FromStandardIdentifier).IsRequired();
            Property(x => x.ToStandardIdentifier).IsRequired();

            HasRequired(a => a.FromStandard).WithMany(b => b.OutgoingConnections).HasForeignKey(a => a.FromStandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.ToStandard).WithMany(b => b.IncomingConnections).HasForeignKey(a => a.ToStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
