using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class VResponseConfiguration : EntityTypeConfiguration<VResponse>
    {
        public VResponseConfiguration()
        {
            ToTable("VResponse", "surveys");
            HasKey(x => new { x.ResponseSessionIdentifier });

            Property(x => x.GroupIdentifier);
            Property(x => x.LastAnsweredQuestionIdentifier);
            Property(x => x.LastChangeUser).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.PeriodIdentifier);
            Property(x => x.RespondentUserIdentifier).IsRequired();
            Property(x => x.ResponseSessionIdentifier).IsRequired();
            Property(x => x.SurveyFormIdentifier).IsRequired();
            Property(x => x.GroupName).IsUnicode(false).HasMaxLength(90);
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.PeriodName).IsUnicode(false).HasMaxLength(50);
            Property(x => x.RespondentLanguage).IsUnicode(false).HasMaxLength(2);
            Property(x => x.RespondentName).IsUnicode(false).HasMaxLength(120);
            Property(x => x.ResponseSessionStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SurveyName).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.ResponseIsLocked).IsRequired();
            Property(x => x.LastChangeTime).IsRequired();
            Property(x => x.ResponseSessionCompleted);
            Property(x => x.ResponseSessionCreated);
            Property(x => x.ResponseSessionStarted);
        }
    }
}
