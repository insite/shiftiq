using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class TTaxConfiguration : EntityTypeConfiguration<TTax>
    {
        public TTaxConfiguration() : this("billing") { }

        public TTaxConfiguration(string schema)
        {
            ToTable("TTax", schema);

            HasKey(x => x.TaxIdentifier);

            Property(x => x.CountryCode).IsRequired().IsUnicode(false).HasMaxLength(2);
            Property(x => x.RegionCode).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.RegionCode).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.TaxRate).HasPrecision(5, 4);
        }
    }
}
