using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class GroupHierarchyConfiguration : EntityTypeConfiguration<GroupHierarchy>
    {
        public GroupHierarchyConfiguration() : this("contacts") { }

        public GroupHierarchyConfiguration(string schema)
        {
            ToTable(schema + ".GroupHierarchy");
            HasKey(x => new { x.GroupIdentifier });
        
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.GroupType).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ParentGroupIdentifier).IsOptional();
            Property(x => x.PathDepth).IsOptional();
            Property(x => x.PathIndent).IsOptional().IsUnicode(false).HasMaxLength(6);
            Property(x => x.PathName).IsOptional().IsUnicode(true);
            Property(x => x.OrganizationIdentifier).IsOptional();
        }
    }
}
