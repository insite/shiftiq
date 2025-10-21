using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QResponseSessionConfiguration : EntityTypeConfiguration<QResponseSession>
    {
        public QResponseSessionConfiguration() : this("surveys") { }

        public QResponseSessionConfiguration(string schema)
        {
            ToTable(schema + ".QResponseSession");
            HasKey(x => new { x.ResponseSessionIdentifier });

            Property(x => x.RespondentLanguage).IsOptional().IsUnicode(false).HasMaxLength(2);
            Property(x => x.RespondentUserIdentifier).IsRequired();
            Property(x => x.ResponseIsLocked).IsRequired();
            Property(x => x.ResponseSessionCompleted).IsOptional();
            Property(x => x.ResponseSessionCreated).IsOptional();
            Property(x => x.ResponseSessionIdentifier).IsRequired();
            Property(x => x.ResponseSessionStarted).IsOptional();
            Property(x => x.ResponseSessionStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.SurveyFormIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.LastChangeType).IsRequired().IsUnicode(false).HasMaxLength(100);

            HasRequired(a => a.Respondent).WithMany(b => b.ResponseSessions).HasForeignKey(c => c.RespondentUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.SurveyForm).WithMany(b => b.QResponseSessions).HasForeignKey(c => c.SurveyFormIdentifier).WillCascadeOnDelete(false);
        }
    }
}