using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VEventGroupPermissionConfiguration : EntityTypeConfiguration<VEventGroupPermission>
    {
        public VEventGroupPermissionConfiguration() : this("contacts") { }

        public VEventGroupPermissionConfiguration(string schema)
        {
            ToTable(schema + ".VEventGroupPermission");
            HasKey(x => new { x.PermissionIdentifier });

            Property(x => x.AllowAdministrate).IsRequired();
            Property(x => x.AllowConfigure).IsRequired();
            Property(x => x.AllowCreate).IsRequired();
            Property(x => x.AllowDelete).IsRequired();
            Property(x => x.AllowExecute).IsRequired();
            Property(x => x.AllowRead).IsRequired();
            Property(x => x.AllowWrite).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.ObjectIdentifier).IsRequired();
            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.PermissionGranted).IsOptional();
            Property(x => x.PermissionGrantedBy).IsOptional();
            Property(x => x.PermissionIdentifier).IsRequired();
            Property(x => x.PermissionMask).IsRequired();

            HasRequired(a => a.Group).WithMany(b => b.EventGroupPermissions).HasForeignKey(c => c.GroupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Event).WithMany(b => b.EventGroupPermissions).HasForeignKey(c => c.ObjectIdentifier).WillCascadeOnDelete(false);
        }
    }
}
