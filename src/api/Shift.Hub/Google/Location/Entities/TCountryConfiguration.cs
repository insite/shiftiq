using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Hub.Google
{
    public class TCountryConfiguration : IEntityTypeConfiguration<TCountry>
    {
        public void Configure(EntityTypeBuilder<TCountry> builder)
        {
            builder.ToTable("country");
            builder.HasKey(x => new { x.CountryCode });

            builder.Property(x => x.CountryCode).HasColumnName("country_code").IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CountryName).HasColumnName("country_name").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.CapitalCityName).HasColumnName("capital_city_name").IsUnicode(false).HasMaxLength(60);
            builder.Property(x => x.ContinentCode).HasColumnName("continent_code").IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").IsUnicode(false).HasMaxLength(3);
            builder.Property(x => x.CurrencyName).HasColumnName("currency_name").IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Languages).HasColumnName("languages").IsUnicode(false).HasMaxLength(60);
            builder.Property(x => x.TopLevelDomain).HasColumnName("top_level_domain").IsUnicode(false).HasMaxLength(3);
            builder.Property(x => x.CountryId).HasColumnName("country_id").IsRequired().HasDefaultValueSql("NEWID()");
        }
    }
}
