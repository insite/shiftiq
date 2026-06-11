using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class QGroupConnectionConfiguration : EntityTypeConfiguration<QGroupConnection>
    {
        public QGroupConnectionConfiguration() : this("contacts") { }

        public QGroupConnectionConfiguration(string schema)
        {
            ToTable(schema + ".QGroupConnection");
            HasKey(x => new { x.ChildGroupIdentifier, x.ParentGroupIdentifier });

            HasRequired(a => a.ParentGroup).WithMany(b => b.ConnectionChildren).HasForeignKey(c => c.ParentGroupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.ChildGroup).WithMany(b => b.ConnectionParents).HasForeignKey(c => c.ChildGroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
