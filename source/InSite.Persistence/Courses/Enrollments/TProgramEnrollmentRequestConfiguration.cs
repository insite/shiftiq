using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramEnrollmentRequestConfiguration : EntityTypeConfiguration<TProgramEnrollmentRequest>
    {
        public TProgramEnrollmentRequestConfiguration() : this("records") { }

        public TProgramEnrollmentRequestConfiguration(string schema)
        {
            ToTable(schema + ".TProgramEnrollmentRequest");
            HasKey(x => new { x.ProgramEnrollmentRequestIdentifier });

            Property(x => x.ProgramEnrollmentRequestIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.LearnerUserIdentifier).IsRequired();
            Property(x => x.LearnerUserEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.RequestFundingPath).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.RequestStatus).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.RequestSubmittedAt).IsRequired();
            Property(x => x.RequestDecidedAt).IsOptional();
            Property(x => x.RequestDecidedBy).IsOptional();
            Property(x => x.RequestFee).IsOptional().HasPrecision(7, 2);
            Property(x => x.RequestFunderEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.ProgramEnrollmentIdentifier).IsOptional();
        }
    }
}
