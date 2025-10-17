using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Api.Location
{
    public class TProvinceConfiguration : IEntityTypeConfiguration<TProvince>
    {
        public void Configure(EntityTypeBuilder<TProvince> builder)
        {
            builder.ToTable("TProvince", "contact");
            builder.HasKey(x => new { x.ProvinceName, x.CountryCode });

            builder.Property(x => x.CountryCode).IsRequired().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.ProvinceCode).IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.ProvinceName).IsRequired().IsUnicode(false).HasMaxLength(80);
            builder.Property(x => x.ProvinceNameTranslation).IsUnicode(false).HasMaxLength(200);
        }
    }
}