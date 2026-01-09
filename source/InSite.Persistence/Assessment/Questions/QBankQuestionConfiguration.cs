using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankQuestionConfiguration : EntityTypeConfiguration<QBankQuestion>
    {
        public QBankQuestionConfiguration() : this("banks") { }

        public QBankQuestionConfiguration(string schema)
        {
            ToTable(schema + ".QBankQuestion");
            HasKey(x => new { x.QuestionIdentifier });

            Property(x => x.BankIdentifier).IsRequired();
            Property(x => x.BankIndex).IsRequired();
            Property(x => x.CompetencyIdentifier).IsOptional();
            Property(x => x.QuestionCode).IsOptional().IsUnicode(false);
            Property(x => x.QuestionDifficulty).IsOptional();
            Property(x => x.QuestionFlag).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.QuestionIdentifier).IsRequired();
            Property(x => x.QuestionLikeItemGroup).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.QuestionReference).IsOptional().IsUnicode(false);
            Property(x => x.QuestionCondition).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuestionTag).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.QuestionTags).IsOptional().IsUnicode(false);
            Property(x => x.QuestionTaxonomy).IsOptional();
            Property(x => x.QuestionText).IsOptional().IsUnicode(false);
            Property(x => x.SetIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.QuestionAssetNumber).IsRequired().IsUnicode(false).HasMaxLength(16);
            Property(x => x.QuestionSourceAssetNumber).IsUnicode(false).HasMaxLength(16);
            Property(x => x.LastChangeType).IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsUnicode(false).HasMaxLength(100);
            Property(x => x.QuestionType).IsOptional().IsUnicode(false).HasMaxLength(21);
            Property(x => x.QuestionPublicationStatus).IsUnicode(false).HasMaxLength(12);

            HasRequired(a => a.Bank).WithMany(b => b.Questions).HasForeignKey(c => c.BankIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Competency).WithMany(b => b.Questions).HasForeignKey(c => c.CompetencyIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Rubric).WithMany(b => b.BankQuestions).HasForeignKey(c => c.RubricIdentifier);
        }
    }
}
