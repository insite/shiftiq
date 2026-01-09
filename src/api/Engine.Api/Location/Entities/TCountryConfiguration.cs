using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Api.Location
{
    public class TCountryConfiguration : IEntityTypeConfiguration<TCountry>
    {
        public void Configure(EntityTypeBuilder<TCountry> builder)
        {
            builder.ToTable("TCountry", "contact");
            builder.HasKey(x => new { x.CountryCode });

            builder.Property(x => x.CapitalCityName).IsUnicode(false).HasMaxLength(60);
            builder.Property(x => x.ContinentCode).IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CountryCode).IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CountryName).IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.CurrencyCode).IsUnicode(false).HasMaxLength(3);
            builder.Property(x => x.CurrencyName).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.Languages).IsUnicode(false).HasMaxLength(60);
            builder.Property(x => x.TopLevelDomain).IsUnicode(false).HasMaxLength(3);
        }
    }
}