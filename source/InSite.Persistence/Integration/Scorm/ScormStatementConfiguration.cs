using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TScormStatementConfiguration : EntityTypeConfiguration<TScormStatement>
    {
        public TScormStatementConfiguration() : this("integration") { }

        public TScormStatementConfiguration(string schema)
        {
            ToTable(schema + ".TScormStatement");
            HasKey(x => new { x.StatementIdentifier });

            Property(x => x.ActorName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ObjectDefinitionName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.StatementData).IsRequired().IsUnicode(false);
            Property(x => x.StatementIdentifier).IsRequired();
            Property(x => x.VerbDisplay).IsOptional().IsUnicode(false).HasMaxLength(100);
        }
    }
}
