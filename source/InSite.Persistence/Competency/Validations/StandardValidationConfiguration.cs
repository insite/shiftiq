using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardValidationConfiguration : EntityTypeConfiguration<StandardValidation>
    {
        public StandardValidationConfiguration() : this("standards") { }

        public StandardValidationConfiguration(string schema)
        {
            ToTable(schema + ".StandardValidation");
            HasKey(x => new { x.ValidationIdentifier });

            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.Expired).IsOptional();
            Property(x => x.IsValidated).IsRequired();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.Notified).IsOptional();
            Property(x => x.SelfAssessmentDate).IsOptional();
            Property(x => x.SelfAssessmentStatus).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.StandardIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.ValidationComment).IsOptional().IsUnicode(false);
            Property(x => x.ValidationDate).IsOptional();
            Property(x => x.ValidationIdentifier).IsRequired();
            Property(x => x.ValidationStatus).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.ValidatorUserIdentifier).IsOptional();

            HasRequired(a => a.Standard).WithMany(b => b.StandardValidations).HasForeignKey(a => a.StandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.UserValidations).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Validator).WithMany(b => b.ValidatorValidations).HasForeignKey(a => a.ValidatorUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
