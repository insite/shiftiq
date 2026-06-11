using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Hub.Google
{
    public class TProvinceConfiguration : IEntityTypeConfiguration<TProvince>
    {
        public void Configure(EntityTypeBuilder<TProvince> builder)
        {
            builder.ToTable("province");
            builder.HasKey(x => new { x.ProvinceName, x.CountryCode });

            builder.Property(x => x.ProvinceName).HasColumnName("province_name").IsRequired().IsUnicode(false).HasMaxLength(80);
            builder.Property(x => x.ProvinceNameTranslations).HasColumnName("province_name_translations").IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.ProvinceCode).HasColumnName("province_code").IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.CountryCode).HasColumnName("country_code").IsRequired().IsUnicode(false).HasMaxLength(2);
        }
    }
}
