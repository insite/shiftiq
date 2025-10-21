using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyRespondentConfiguration : EntityTypeConfiguration<QSurveyRespondent>
    {
        public QSurveyRespondentConfiguration() : this("surveys") { }

        public QSurveyRespondentConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyRespondent");
            HasKey(x => new { x.SurveyFormIdentifier, x.RespondentUserIdentifier });

            Property(x => x.RespondentUserIdentifier).IsRequired();
            Property(x => x.SurveyFormIdentifier).IsRequired();

            HasRequired(a => a.SurveyForm).WithMany(b => b.QSurveyRespondents).HasForeignKey(c => c.SurveyFormIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Respondent).WithMany(b => b.SurveyRespondents).HasForeignKey(c => c.RespondentUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
