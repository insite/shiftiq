using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class VStatementConfiguration : EntityTypeConfiguration<VStatement>
    {
        public VStatementConfiguration() : this("records") { }

        public VStatementConfiguration(string schema)
        {
            ToTable(schema + ".VStatement");
            HasKey(x => new { x.StatementIdentifier });
            Property(x => x.ActorEmail).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ActorName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ObjectId).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.StatementData).IsRequired().IsUnicode(true);
            Property(x => x.StatementIdentifier).IsRequired();
            Property(x => x.StatementProvider).IsRequired().IsUnicode(true).HasMaxLength(100);
            Property(x => x.StatementTime).IsRequired();
            Property(x => x.VerbDisplay).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}
