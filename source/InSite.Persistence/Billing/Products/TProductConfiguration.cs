using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class TProductConfiguration : EntityTypeConfiguration<TProduct>
    {
        public TProductConfiguration() : this("billing") { }

        public TProductConfiguration(string schema)
        {
            ToTable(schema + ".TProduct");
            HasKey(x => new { x.ProductIdentifier });
            Property(x => x.ProductIdentifier).IsRequired();
            Property(x => x.ProductName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProductDescription).IsOptional().IsUnicode(false).HasMaxLength(2000);
            Property(x => x.ProductType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ProductImageUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.ObjectType).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ObjectIdentifier).IsOptional();
            Property(x => x.Published).IsOptional();
            Property(x => x.CreatedBy).IsOptional();
            Property(x => x.PublishedBy).IsOptional();
            Property(x => x.ModifiedBy).IsOptional();
            Property(x => x.ProductPrice).IsOptional().HasPrecision(9, 2);
            Property(x => x.ProductUrl).IsOptional().IsUnicode(false).HasMaxLength(2048);
            Property(x => x.ProductSummary).IsOptional().IsUnicode(false).HasMaxLength(500);
        }
    }
}
