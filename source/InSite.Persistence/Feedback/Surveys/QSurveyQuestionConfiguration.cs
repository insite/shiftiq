using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyQuestionConfiguration : EntityTypeConfiguration<QSurveyQuestion>
    {
        public QSurveyQuestionConfiguration() : this("surveys") { }

        public QSurveyQuestionConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyQuestion");
            HasKey(x => new { x.SurveyQuestionIdentifier });
            Property(x => x.SurveyFormIdentifier).IsRequired();
            Property(x => x.SurveyQuestionAttribute).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.SurveyQuestionCode).IsOptional().IsUnicode(false).HasMaxLength(4);
            Property(x => x.SurveyQuestionIdentifier).IsRequired();
            Property(x => x.SurveyQuestionIndicator).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.SurveyQuestionIsRequired).IsRequired();
            Property(x => x.SurveyQuestionListEnableBranch).IsRequired();
            Property(x => x.SurveyQuestionListEnableGroupMembership).IsRequired();
            Property(x => x.SurveyQuestionListEnableOtherText).IsRequired();
            Property(x => x.SurveyQuestionListEnableRandomization).IsRequired();
            Property(x => x.SurveyQuestionNumberEnableStatistics).IsRequired();
            Property(x => x.SurveyQuestionSequence).IsRequired();
            Property(x => x.SurveyQuestionTextCharacterLimit).IsOptional();
            Property(x => x.SurveyQuestionTextLineCount).IsOptional();
            Property(x => x.SurveyQuestionType).IsRequired().IsUnicode(false).HasMaxLength(20);

            HasRequired(a => a.SurveyForm).WithMany(b => b.QSurveyQuestions).HasForeignKey(c => c.SurveyFormIdentifier).WillCascadeOnDelete(false);
        }
    }
}
