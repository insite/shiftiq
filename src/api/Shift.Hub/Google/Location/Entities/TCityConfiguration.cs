using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Hub.Google
{
    public class TCityConfiguration : IEntityTypeConfiguration<TCity>
    {
        public void Configure(EntityTypeBuilder<TCity> builder)
        {
            builder.ToTable("city");
            builder.HasKey(x => new { x.CityName, x.ProvinceCode, x.CountryCode });

            builder.Property(x => x.CityName).HasColumnName("city_name").IsRequired().IsUnicode(false).HasMaxLength(60);
            builder.Property(x => x.ProvinceCode).HasColumnName("province_code").IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CountryCode).HasColumnName("country_code").IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.Latitude).HasColumnName("latitude").IsRequired().HasPrecision(9, 6);
            builder.Property(x => x.Longitude).HasColumnName("longitude").IsRequired().HasPrecision(9, 6);
        }
    }
}
