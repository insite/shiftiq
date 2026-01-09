using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Integration.BCMail
{
    public class ExamDistributionRequestConfiguration : EntityTypeConfiguration<ExamDistributionRequest>
    {
        public ExamDistributionRequestConfiguration() : this("custom_ita") { }

        public ExamDistributionRequestConfiguration(string schema)
        {
            ToTable(schema + ".ExamDistributionRequest");
            HasKey(x => new { x.RequestIdentifier });

            Property(x => x.JobCode).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.JobErrors).IsOptional().IsUnicode(false);
            Property(x => x.JobStatus).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.Requested).IsRequired();
            Property(x => x.RequestedBy).IsRequired();
            Property(x => x.RequestIdentifier).IsRequired();
        }
    }
}
