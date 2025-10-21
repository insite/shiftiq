using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    class QBankQuestionGradeItemConfiguration : EntityTypeConfiguration<QBankQuestionGradeItem>
    {
        public QBankQuestionGradeItemConfiguration() : this("banks") { }

        public QBankQuestionGradeItemConfiguration(string schema)
        {
            ToTable(schema + ".QBankQuestionGradeItem");
            HasKey(x => new { x.QuestionIdentifier, x.FormIdentifier });

            Property(x => x.QuestionIdentifier).IsRequired();
            Property(x => x.FormIdentifier).IsRequired();

            HasRequired(a => a.Form).WithMany(b => b.GradeItems).HasForeignKey(c => c.FormIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Question).WithMany(b => b.GradeItems).HasForeignKey(c => c.QuestionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
