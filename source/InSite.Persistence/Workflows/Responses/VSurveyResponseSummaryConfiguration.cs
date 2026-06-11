using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class VSurveyResponseSummaryConfiguration : EntityTypeConfiguration<VSurveyResponseSummary>
    {
        public VSurveyResponseSummaryConfiguration() : this("surveys") { }

        public VSurveyResponseSummaryConfiguration(string schema)
        {
            ToTable(schema + ".VSurveyResponseSummary");
            HasKey(x => new { x.SurveyFormIdentifier });

            Property(x => x.SurveyFormIdentifier).IsRequired();

            Property(x => x.MinResponseStarted).IsOptional();
            Property(x => x.MaxResponseCompleted).IsOptional();
            Property(x => x.AvgResponseTimeTaken).IsOptional();
            Property(x => x.SumResponseStartCount).IsRequired();
            Property(x => x.SumResponseCompleteCount).IsRequired();
        }
    }
}