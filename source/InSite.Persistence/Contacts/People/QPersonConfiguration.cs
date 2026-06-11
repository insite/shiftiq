using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class QPersonConfiguration : EntityTypeConfiguration<QPerson>
    {
        public QPersonConfiguration()
        {
            ToTable("QPerson", "contacts");
            HasKey(x => new { x.PersonIdentifier });

            Property(x => x.AccessRevokedBy).IsUnicode(false).HasMaxLength(254);
            Property(x => x.AgeGroup).IsUnicode(false).HasMaxLength(20);
            Property(x => x.CandidateLinkedInUrl).IsUnicode(false).HasMaxLength(500);
            Property(x => x.CandidateOccupationList).IsUnicode(false);
            Property(x => x.Citizenship).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ConsentConsultation).IsUnicode(false).HasMaxLength(30);
            Property(x => x.ConsentToShare).IsUnicode(false).HasMaxLength(30);
            Property(x => x.CredentialingCountry).IsUnicode(false).HasMaxLength(100);
            Property(x => x.EducationLevel).IsUnicode(false).HasMaxLength(80);
            Property(x => x.EmergencyContactName).IsUnicode(false).HasMaxLength(100);
            Property(x => x.EmergencyContactPhone).IsUnicode(false).HasMaxLength(32);
            Property(x => x.EmergencyContactRelationship).IsUnicode(false).HasMaxLength(50);
            Property(x => x.EmployeeType).IsUnicode(false).HasMaxLength(16);
            Property(x => x.EmployeeUnion).IsUnicode(false).HasMaxLength(32);
            Property(x => x.FirstLanguage).IsUnicode(false).HasMaxLength(20);
            Property(x => x.FullName).IsUnicode(false).HasMaxLength(120);
            Property(x => x.Gender).IsUnicode(false).HasMaxLength(20);
            Property(x => x.ImmigrationApplicant).IsUnicode(false).HasMaxLength(20);
            Property(x => x.ImmigrationCategory).IsUnicode(false).HasMaxLength(120);
            Property(x => x.ImmigrationDestination).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ImmigrationDisability).IsUnicode(false).HasMaxLength(20);
            Property(x => x.ImmigrationNumber).IsUnicode(false).HasMaxLength(64);
            Property(x => x.JobsApprovedBy).IsUnicode(false).HasMaxLength(128);
            Property(x => x.JobTitle).IsUnicode(false).HasMaxLength(256);
            Property(x => x.Language).IsUnicode(false).HasMaxLength(2);
            Property(x => x.PersonCode).IsUnicode(false).HasMaxLength(20);
            Property(x => x.Phone).IsUnicode(false).HasMaxLength(30);
            Property(x => x.PhoneFax).IsUnicode(false).HasMaxLength(32);
            Property(x => x.PhoneHome).IsUnicode(false).HasMaxLength(32);
            Property(x => x.PhoneOther).IsUnicode(false).HasMaxLength(32);
            Property(x => x.PhoneWork).IsUnicode(false).HasMaxLength(32);
            Property(x => x.Referrer).IsUnicode(false).HasMaxLength(100);
            Property(x => x.ReferrerOther).IsUnicode(false).HasMaxLength(200);
            Property(x => x.Region).IsUnicode(false).HasMaxLength(50);
            Property(x => x.ShippingPreference).IsUnicode(false).HasMaxLength(20);
            Property(x => x.SocialInsuranceNumber).IsUnicode(false).HasMaxLength(32);
            Property(x => x.TradeworkerNumber).IsUnicode(false).HasMaxLength(20);
            Property(x => x.UserAccessGrantedBy).IsUnicode(false).HasMaxLength(128);
            Property(x => x.UserApproveReason).IsUnicode(false).HasMaxLength(200);
            Property(x => x.WebSiteUrl).IsUnicode(false).HasMaxLength(500);

            HasOptional(a => a.BillingAddress).WithMany(b => b.BillingPersons).HasForeignKey(c => c.BillingAddressIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.HomeAddress).WithMany(b => b.HomePersons).HasForeignKey(c => c.HomeAddressIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.ShippingAddress).WithMany(b => b.ShippingPersons).HasForeignKey(c => c.ShippingAddressIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.WorkAddress).WithMany(b => b.WorkPersons).HasForeignKey(c => c.WorkAddressIdentifier).WillCascadeOnDelete(false);

            HasRequired(a => a.User).WithMany(b => b.Persons).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Organization).WithMany(b => b.Persons).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.EmployerGroup).WithMany(b => b.EmployeePersons).HasForeignKey(c => c.EmployerGroupIdentifier).WillCascadeOnDelete(false);
        }
    }
}
