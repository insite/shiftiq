using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardContainmentSummaryConfiguration : EntityTypeConfiguration<StandardContainmentSummary>
    {
        public StandardContainmentSummaryConfiguration() : this("standards") { }

        public StandardContainmentSummaryConfiguration(string schema)
        {
            ToTable(schema + ".StandardContainmentSummary");
            HasKey(x => new { x.ChildStandardIdentifier });
        
            Property(x => x.ChildAssetNumber).IsRequired();
            Property(x => x.ChildIcon).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ChildName).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ChildSequence).IsRequired();
            Property(x => x.ChildStandardIdentifier).IsRequired();
            Property(x => x.ChildStandardType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.ChildOrganizationIdentifier).IsRequired();
            Property(x => x.ChildTitle).IsOptional().IsUnicode(true).HasMaxLength(100);
            Property(x => x.ParentAssetNumber).IsRequired();
            Property(x => x.ParentIcon).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ParentIsPrimaryContainer).IsOptional();
            Property(x => x.ParentName).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ParentSequence).IsRequired();
            Property(x => x.ParentStandardIdentifier).IsRequired();
            Property(x => x.ParentStandardType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.ParentOrganizationIdentifier).IsRequired();
            Property(x => x.ParentTitle).IsOptional().IsUnicode(true).HasMaxLength(100);
        }
    }
}
