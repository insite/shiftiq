using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class UserCompetencyConfiguration : EntityTypeConfiguration<UserCompetency>
    {
        public UserCompetencyConfiguration() : this("custom_cmds") { }

        public UserCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".UserCompetency");
            HasKey(x => new { x.CompetencyStandardIdentifier, x.UserIdentifier });
        
            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.ExpirationDate).IsOptional();
            Property(x => x.IsModuleQuizCompleted).IsRequired();
            Property(x => x.IsValidated).IsRequired();
            Property(x => x.Notified).IsOptional();
            Property(x => x.SelfAssessmentDate).IsOptional();
            Property(x => x.SelfAssessmentStatus).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.ValidationComment).IsOptional().IsUnicode(false);
            Property(x => x.ValidationDate).IsOptional();
            Property(x => x.ValidationStatus).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ValidatorUserIdentifier).IsOptional();
        }
    }
}
