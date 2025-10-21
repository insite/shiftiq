using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TLtiLinkConfiguration : EntityTypeConfiguration<TLtiLink>
    {
        public TLtiLinkConfiguration() : this("courses") { }

        public TLtiLinkConfiguration(string schema)
        {
            ToTable(schema + ".TLtiLink");
            HasKey(x => new { x.LinkIdentifier });
            Property(x => x.AssetNumber).IsRequired();
            Property(x => x.LinkIdentifier).IsRequired();
            Property(x => x.ResourceCode).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ResourceName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ResourceParameters).IsOptional().IsUnicode(false);
            Property(x => x.ResourceSummary).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ResourceTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ToolConsumerKey).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ToolConsumerSecret).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ToolProviderName).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ToolProviderType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ToolProviderUrl).IsRequired().IsUnicode(false).HasMaxLength(500);
        }
    }
}
