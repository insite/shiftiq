using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TActionConfiguration : EntityTypeConfiguration<TAction>
    {
        public TActionConfiguration() : this("settings") { }

        public TActionConfiguration(string schema)
        {
            ToTable(schema + ".TAction");
            HasKey(x => new { x.ActionIdentifier });

            Property(x => x.ActionIcon).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ActionIdentifier).IsRequired();
            Property(x => x.ActionList).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ActionName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ActionNameShort).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ActionType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ActionUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.AuthorizationRequirement).IsOptional().IsUnicode(false).HasMaxLength(34);
            Property(x => x.AuthorityType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ControllerPath).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ExtraBreadcrumb).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.HelpUrl).IsOptional().IsUnicode(false).HasMaxLength(500);

            Property(x => x.NavigationParentActionIdentifier).IsOptional();
            Property(x => x.PermissionParentActionIdentifier).IsOptional();

            HasOptional(a => a.NavigationParent).WithMany(b => b.NavigationChildren).HasForeignKey(c => c.NavigationParentActionIdentifier);
            HasOptional(a => a.PermissionParent).WithMany(b => b.PermissionChildren).HasForeignKey(c => c.PermissionParentActionIdentifier);
        }
    }
}