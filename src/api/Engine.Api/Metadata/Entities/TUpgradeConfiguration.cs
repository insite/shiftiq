using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Api.Metadata
{
    public class TUpgradeConfiguration : IEntityTypeConfiguration<TUpgrade>
    {
        public void Configure(EntityTypeBuilder<TUpgrade> builder)
        {
            builder.ToTable("TUpgrade", "database");
            builder.HasKey(x => new { x.ScriptName });

            builder.Property(x => x.ScriptData).IsUnicode(false);
            builder.Property(x => x.ScriptExecuted).IsRequired();
            builder.Property(x => x.ScriptName).IsRequired().IsUnicode(false).HasMaxLength(128);
        }
    }
}