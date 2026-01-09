using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Api.Translation
{
    public class TTranslationConfiguration : IEntityTypeConfiguration<TTranslation>
    {
        public void Configure(EntityTypeBuilder<TTranslation> builder)
        {
            builder.ToTable("TTranslation", "content");
            builder.HasKey(x => new { x.TranslationIdentifier });

            builder.Property(x => x.ar).IsUnicode(true);
            builder.Property(x => x.de).IsUnicode(true);
            builder.Property(x => x.en).IsUnicode(true);
            builder.Property(x => x.eo).IsUnicode(true);
            builder.Property(x => x.es).IsUnicode(true);
            builder.Property(x => x.fr).IsUnicode(true);
            builder.Property(x => x.he).IsUnicode(true);
            builder.Property(x => x.it).IsUnicode(true);
            builder.Property(x => x.ja).IsUnicode(true);
            builder.Property(x => x.ko).IsUnicode(true);
            builder.Property(x => x.la).IsUnicode(true);
            builder.Property(x => x.nl).IsUnicode(true);
            builder.Property(x => x.no).IsUnicode(true);
            builder.Property(x => x.pa).IsUnicode(true);
            builder.Property(x => x.pl).IsUnicode(true);
            builder.Property(x => x.pt).IsUnicode(true);
            builder.Property(x => x.ru).IsUnicode(true);
            builder.Property(x => x.sv).IsUnicode(true);
            builder.Property(x => x.TimestampCreated).IsRequired();
            builder.Property(x => x.TimestampExpired);
            builder.Property(x => x.TimestampModified).IsRequired();
            builder.Property(x => x.TranslationIdentifier).IsRequired();
            builder.Property(x => x.uk).IsUnicode(true);
            builder.Property(x => x.zh).IsUnicode(true);
        }
    }
}
