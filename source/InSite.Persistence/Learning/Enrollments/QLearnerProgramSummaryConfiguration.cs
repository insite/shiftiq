using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class QLearnerProgramSummaryConfiguration : EntityTypeConfiguration<QLearnerProgramSummary>
    {
        public QLearnerProgramSummaryConfiguration() : this("reports") { }

        public QLearnerProgramSummaryConfiguration(string schema)
        {
            ToTable(schema + ".QLearnerProgramSummary");
            HasKey(x => new { x.SummaryIdentifier });

            Property(x => x.AsAt).IsRequired();
            Property(x => x.ImmigrationArrivalDate).IsOptional();
            Property(x => x.ImmigrationArrivalStatus).IsOptional().IsUnicode(false).HasMaxLength(4);
            Property(x => x.LearnerAccessGranted).IsOptional();
            Property(x => x.LearnerAccountCreated).IsOptional();
            Property(x => x.LearnerAddedToProgram).IsOptional();
            Property(x => x.LearnerEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.LearnerGender).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LearnerName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.LearnerStreams).IsOptional().IsUnicode(true).HasMaxLength(200);
            Property(x => x.ProgramGradeItems).IsOptional();
            Property(x => x.ProgramGradeItemsCompleted).IsOptional();
            Property(x => x.ProgramName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ReferrerIndustry).IsOptional().IsUnicode(true);
            Property(x => x.ReferrerName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ReferrerNameOther).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ReferrerProvince).IsOptional().IsUnicode(true);
            Property(x => x.ReferrerRole).IsOptional().IsUnicode(true).HasMaxLength(100);
            Property(x => x.SummaryIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(a => a.User).WithMany(b => b.QLearnerProgramSummaries).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
