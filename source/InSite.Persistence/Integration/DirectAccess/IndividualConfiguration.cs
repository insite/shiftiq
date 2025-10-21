using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public class IndividualConfiguration : EntityTypeConfiguration<Individual>
    {
        public IndividualConfiguration() : this("custom_ita") { }

        public IndividualConfiguration(string schema)
        {
            ToTable(schema + ".Individual");
            HasKey(x => new { x.IndividualKey });

            Property(e => e.IndividualKey).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.AboriginalIdentity).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AboriginalIndicator).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AddressCity).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AddressLine1).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AddressLine2).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AddressPostalCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AddressProvince).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.Birthdate).IsOptional();
            Property(x => x.ContactIdentifier).IsOptional();
            Property(x => x.CrmIdentifier).IsOptional();
            Property(x => x.Email).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.FirstName).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.Gender).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.HashCode).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.IsActive).IsRequired();
            Property(x => x.IsDeceased).IsRequired();
            Property(x => x.IsMerged).IsRequired();
            Property(x => x.IsNew).IsRequired();
            Property(x => x.LastName).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.MiddleName).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.Mobile).IsOptional().IsUnicode(false).HasMaxLength(15);
            Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.PersonalEducationNumber).IsOptional();
            Property(x => x.Phone).IsOptional().IsUnicode(false).HasMaxLength(15);
            Property(x => x.ProgramType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.Refreshed).IsRequired();
            Property(x => x.RefreshedBy).IsRequired();
        }
    }
}