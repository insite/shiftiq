using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class TLearnerAttemptSummaryConfiguration : EntityTypeConfiguration<TLearnerAttemptSummary>
    {
        public TLearnerAttemptSummaryConfiguration() : this("assessments") { }

        public TLearnerAttemptSummaryConfiguration(string schema)
        {
            ToTable(schema + ".TLearnerAttemptSummary");
            HasKey(x => new { x.FormIdentifier, x.LearnerUserIdentifier });
        }
    }
}
