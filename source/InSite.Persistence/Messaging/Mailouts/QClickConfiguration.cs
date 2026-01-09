using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QClickConfiguration : EntityTypeConfiguration<QClick>
    {
        public QClickConfiguration() : this("messages") { }

        public QClickConfiguration(string schema)
        {
            ToTable(schema + ".QClick");
            HasKey(x => x.ClickIdentifier);

            Property(x => x.Clicked).IsRequired();
            Property(x => x.LinkIdentifier).IsRequired();
            Property(x => x.UserBrowser).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserHostAddress).IsRequired().IsUnicode(false).HasMaxLength(15);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}