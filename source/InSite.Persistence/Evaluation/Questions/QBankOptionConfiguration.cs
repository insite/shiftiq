using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankOptionConfiguration : EntityTypeConfiguration<QBankOption>
    {
        public QBankOptionConfiguration() : this("banks") { }

        public QBankOptionConfiguration(string schema)
        {
            ToTable(schema + ".QBankOption");
            HasKey(x => new { x.QuestionIdentifier, x.OptionKey });

            Property(x => x.OptionText).IsUnicode(false);

            HasRequired(a => a.Question).WithMany(b => b.Options).HasForeignKey(c => c.QuestionIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Competency).WithMany(b => b.Options).HasForeignKey(c => c.CompetencyIdentifier).WillCascadeOnDelete(false);
        }
    }
}
