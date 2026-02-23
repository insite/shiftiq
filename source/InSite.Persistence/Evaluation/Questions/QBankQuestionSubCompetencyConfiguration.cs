using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankQuestionSubCompetencyConfiguration : EntityTypeConfiguration<QBankQuestionSubCompetency>
    {
        public QBankQuestionSubCompetencyConfiguration() : this("banks") { }

        public QBankQuestionSubCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".QBankQuestionSubCompetency");
            HasKey(x => new { x.QuestionIdentifier, x.SubCompetencyIdentifier });

            HasRequired(a => a.Question).WithMany(b => b.SubCompetencies).HasForeignKey(c => c.QuestionIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.SubCompetency).WithMany(b => b.QuestionSubCompetencies).HasForeignKey(c => c.SubCompetencyIdentifier).WillCascadeOnDelete(false);
        }
    }
}
