using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class GroupToolkitPermissionSummaryConfiguration : EntityTypeConfiguration<GroupToolkitPermissionSummary>
    {
        public GroupToolkitPermissionSummaryConfiguration() : this("contacts") { }

        public GroupToolkitPermissionSummaryConfiguration(string schema)
        {
            ToTable(schema + ".GroupToolkitPermissionSummary");
            HasKey(x => new { x.GroupIdentifier, x.ToolkitNumber });
        
            Property(x => x.AllowDelete).IsRequired();
            Property(x => x.AllowFullControl).IsRequired();
            Property(x => x.AllowRead).IsRequired();
            Property(x => x.AllowWrite).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.ToolkitName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ToolkitNumber).IsRequired();
        }
    }
}
