using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contents.Read;

namespace InSite.Persistence
{
    public class TContentConfiguration : EntityTypeConfiguration<TContent>
    {
        public TContentConfiguration() : this("contents") { }

        public TContentConfiguration(string schema)
        {
            ToTable(schema + ".TContent");
            HasKey(x => new { x.ContentIdentifier });

            Property(x => x.ContainerIdentifier).IsRequired();
            Property(x => x.ContainerType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContentHtml).IsOptional().IsUnicode(true);
            Property(x => x.ContentIdentifier).IsRequired();
            Property(x => x.ContentLabel).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContentLanguage).IsRequired().IsUnicode(false).HasMaxLength(2);
            Property(x => x.ContentSequence).IsOptional();
            Property(x => x.ContentSnip).IsRequired().IsUnicode(true).HasMaxLength(100);
            Property(x => x.ContentText).IsOptional().IsUnicode(true);
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}