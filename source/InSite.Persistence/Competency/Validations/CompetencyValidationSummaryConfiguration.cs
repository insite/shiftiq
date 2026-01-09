using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class CompetencyValidationSummaryConfiguration : EntityTypeConfiguration<CompetencyValidationSummary>
    {
        public CompetencyValidationSummaryConfiguration() : this("standards") { }

        public CompetencyValidationSummaryConfiguration(string schema)
        {
            ToTable(schema + ".CompetencyValidationSummary");
            HasKey(x => new { x.ChangeIdentifier });
        
            Property(x => x.CompetencyCode).IsUnicode(false).HasMaxLength(256);
            Property(x => x.UserName).IsUnicode(false).HasMaxLength(145);
            Property(x => x.ChangeComment).IsUnicode(false);
            Property(x => x.ChangeStatus).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ValidatorName).IsUnicode(false).HasMaxLength(145);
        }
    }
}
