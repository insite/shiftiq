using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class ContactExperienceConfiguration : EntityTypeConfiguration<ContactExperience>
    {
        public ContactExperienceConfiguration() : this("contacts") { }

        public ContactExperienceConfiguration(string schema)
        {
            ToTable(schema + ".ContactExperience");
            HasKey(x => new { x.ExperienceIdentifier });
            
            HasRequired(a => a.User).WithMany(b => b.ContactExperiences).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AuthorityCity).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AuthorityCountry).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.AuthorityName).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.AuthorityProvince).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.Completed).IsOptional();
            Property(x => x.ContactExperienceType).IsRequired().IsUnicode(false).HasMaxLength(64);
            Property(x => x.CreditHours).IsOptional();
            Property(x => x.Description).IsOptional().IsUnicode(false);
            Property(x => x.Expired).IsOptional();
            Property(x => x.IsSuccess).IsRequired();
            Property(x => x.LifetimeMonths).IsOptional();
            Property(x => x.Score).IsOptional();
            Property(x => x.Status).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ExperienceIdentifier).IsRequired();
            Property(x => x.Title).IsOptional().IsUnicode(false).HasMaxLength(256);
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
