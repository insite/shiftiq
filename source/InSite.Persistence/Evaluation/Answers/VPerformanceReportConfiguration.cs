using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    internal class VPerformanceReportConfiguration : EntityTypeConfiguration<VPerformanceReport>
    {
        public VPerformanceReportConfiguration() : this("assessments") { }

        public VPerformanceReportConfiguration(string schema)
        {
            ToTable(schema + ".VPerformanceReport");
            HasKey(x => new { x.AttemptIdentifier, x.QuestionIdentifier });
        }
    }
}
