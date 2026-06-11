using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Hub.Google
{
    public class TTranslationConfiguration : IEntityTypeConfiguration<TTranslation>
    {
        public void Configure(EntityTypeBuilder<TTranslation> builder)
        {
            builder.ToTable("translation");
            builder.HasKey(x => new { x.TranslationId });

            builder.Property(x => x.en).HasColumnName("en").IsUnicode(true);
            builder.Property(x => x.ar).HasColumnName("ar").IsUnicode(true);
            builder.Property(x => x.de).HasColumnName("de").IsUnicode(true);
            builder.Property(x => x.eo).HasColumnName("eo").IsUnicode(true);
            builder.Property(x => x.es).HasColumnName("es").IsUnicode(true);
            builder.Property(x => x.fr).HasColumnName("fr").IsUnicode(true);
            builder.Property(x => x.he).HasColumnName("he").IsUnicode(true);
            builder.Property(x => x.it).HasColumnName("it").IsUnicode(true);
            builder.Property(x => x.ja).HasColumnName("ja").IsUnicode(true);
            builder.Property(x => x.ko).HasColumnName("ko").IsUnicode(true);
            builder.Property(x => x.la).HasColumnName("la").IsUnicode(true);
            builder.Property(x => x.nl).HasColumnName("nl").IsUnicode(true);
            builder.Property(x => x.no).HasColumnName("no").IsUnicode(true);
            builder.Property(x => x.pa).HasColumnName("pa").IsUnicode(true);
            builder.Property(x => x.pl).HasColumnName("pl").IsUnicode(true);
            builder.Property(x => x.pt).HasColumnName("pt").IsUnicode(true);
            builder.Property(x => x.ru).HasColumnName("ru").IsUnicode(true);
            builder.Property(x => x.sv).HasColumnName("sv").IsUnicode(true);
            builder.Property(x => x.uk).HasColumnName("uk").IsUnicode(true);
            builder.Property(x => x.zh).HasColumnName("zh").IsUnicode(true);
            builder.Property(x => x.TimestampCreated).HasColumnName("created_at").IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.TimestampModified).HasColumnName("modified_at").IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.TimestampExpired).HasColumnName("expired_at");
            builder.Property(x => x.TranslationId).HasColumnName("translation_id").IsRequired();
        }
    }
}
