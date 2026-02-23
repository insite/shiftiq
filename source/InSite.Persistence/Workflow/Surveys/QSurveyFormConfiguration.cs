using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyFormConfiguration : EntityTypeConfiguration<QSurveyForm>
    {
        public QSurveyFormConfiguration() : this("surveys") { }

        public QSurveyFormConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyForm");
            HasKey(x => new { x.SurveyFormIdentifier });

            Property(x => x.AssetNumber).IsRequired();
            Property(x => x.EnableUserConfidentiality).IsRequired();
            Property(x => x.UserFeedback).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.RequireUserAuthentication).IsRequired();
            Property(x => x.RequireUserIdentification).IsRequired();
            Property(x => x.ResponseLimitPerUser).IsOptional();
            Property(x => x.DisplaySummaryChart).IsRequired();
            Property(x => x.SurveyFormClosed).IsOptional();
            Property(x => x.SurveyFormIdentifier).IsRequired();
            Property(x => x.SurveyFormLanguage).IsOptional().IsUnicode(false).HasMaxLength(2);
            Property(x => x.SurveyFormLanguageTranslations).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SurveyFormLocked).IsOptional();
            Property(x => x.SurveyFormName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.SurveyFormTitle).IsUnicode(false).HasMaxLength(256);
            Property(x => x.SurveyFormOpened).IsOptional();
            Property(x => x.SurveyFormStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SurveyMessageInvitation).IsOptional();
            Property(x => x.SurveyMessageResponseCompleted).IsOptional();
            Property(x => x.SurveyMessageResponseConfirmed).IsOptional();
            Property(x => x.SurveyMessageResponseStarted).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);
        }
    }
}
