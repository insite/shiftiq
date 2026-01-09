using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QLinkConfiguration : EntityTypeConfiguration<QLink>
    {
        public QLinkConfiguration() : this("messages") { }

        public QLinkConfiguration(string schema)
        {
            ToTable(schema + ".QLink");
            HasKey(x => new { x.LinkIdentifier });

            Property(x => x.ClickCount).IsRequired();
            Property(x => x.LinkIdentifier).IsRequired();
            Property(x => x.LinkText).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.LinkUrl).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.LinkUrlHash).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.UserCount).IsRequired();
        }
    }
}