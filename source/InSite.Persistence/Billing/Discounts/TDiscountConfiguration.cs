using System.Data.Entity.ModelConfiguration;

using InSite.Application.Payments.Read;

namespace InSite.Persistence
{
    public class TDiscountConfiguration : EntityTypeConfiguration<TDiscount>
    {
        public TDiscountConfiguration() : this("payments") { }

        public TDiscountConfiguration(string schema)
        {
            ToTable(schema + ".TDiscount");
            HasKey(x => x.DiscountIdentifier);
            Property(x => x.DiscountCode).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.DiscountPercent).IsRequired().HasPrecision(5, 2);
            Property(x => x.DiscountDescription).IsOptional().IsUnicode(false);
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}