using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QResponseAnswerConfiguration : EntityTypeConfiguration<QResponseAnswer>
    {
        public QResponseAnswerConfiguration() : this("surveys") { }

        public QResponseAnswerConfiguration(string schema)
        {
            ToTable(schema + ".QResponseAnswer");
            HasKey(x => new { x.ResponseSessionIdentifier,x.SurveyQuestionIdentifier });

            Property(x => x.RespondentUserIdentifier).IsRequired();
            Property(x => x.ResponseAnswerText).IsOptional().IsUnicode(true);
            Property(x => x.ResponseSessionIdentifier).IsRequired();
            Property(x => x.SurveyQuestionIdentifier).IsRequired();

            HasRequired(a => a.ResponseSession).WithMany(b => b.QResponseAnswers).HasForeignKey(c => c.ResponseSessionIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.SurveyQuestion).WithMany(b => b.QResponseAnswers).HasForeignKey(c => c.SurveyQuestionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
