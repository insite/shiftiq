using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class PersonConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("QPerson", "contacts");
        builder.HasKey(x => new { x.PersonIdentifier });

        builder.Property(x => x.EmployeeUnion).HasColumnName("EmployeeUnion").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.IsAdministrator).HasColumnName("IsAdministrator").IsRequired();
        builder.Property(x => x.SocialInsuranceNumber).HasColumnName("SocialInsuranceNumber").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.PersonCode).HasColumnName("PersonCode").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.IsLearner).HasColumnName("IsLearner").IsRequired();
        builder.Property(x => x.Birthdate).HasColumnName("Birthdate");
        builder.Property(x => x.Citizenship).HasColumnName("Citizenship").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
        builder.Property(x => x.CustomKey).HasColumnName("CustomKey");
        builder.Property(x => x.EducationLevel).HasColumnName("EducationLevel").IsUnicode(false).HasMaxLength(80);
        builder.Property(x => x.Gender).HasColumnName("Gender").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.ImmigrationApplicant).HasColumnName("ImmigrationApplicant").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.ImmigrationCategory).HasColumnName("ImmigrationCategory").IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.ImmigrationDestination).HasColumnName("ImmigrationDestination").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ImmigrationLandingDate).HasColumnName("ImmigrationLandingDate");
        builder.Property(x => x.ImmigrationNumber).HasColumnName("ImmigrationNumber").IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.JobTitle).HasColumnName("JobTitle").IsUnicode(false).HasMaxLength(256);
        builder.Property(x => x.Language).HasColumnName("Language").IsUnicode(false).HasMaxLength(2);
        builder.Property(x => x.MemberEndDate).HasColumnName("MemberEndDate");
        builder.Property(x => x.MemberStartDate).HasColumnName("MemberStartDate");
        builder.Property(x => x.Modified).HasColumnName("Modified").IsRequired();
        builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired();
        builder.Property(x => x.Phone).HasColumnName("Phone").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.PhoneFax).HasColumnName("PhoneFax").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.PhoneHome).HasColumnName("PhoneHome").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.PhoneOther).HasColumnName("PhoneOther").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.PhoneWork).HasColumnName("PhoneWork").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.Referrer).HasColumnName("Referrer").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ReferrerOther).HasColumnName("ReferrerOther").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.Region).HasColumnName("Region").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CredentialingCountry).HasColumnName("CredentialingCountry").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ShippingPreference).HasColumnName("ShippingPreference").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.TradeworkerNumber).HasColumnName("TradeworkerNumber").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.WebSiteUrl).HasColumnName("WebSiteUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.WelcomeEmailsSentToUser).HasColumnName("WelcomeEmailsSentToUser");
        builder.Property(x => x.EmergencyContactName).HasColumnName("EmergencyContactName").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.EmergencyContactPhone).HasColumnName("EmergencyContactPhone").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.EmergencyContactRelationship).HasColumnName("EmergencyContactRelationship").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.FirstLanguage).HasColumnName("FirstLanguage").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.EmployerGroupIdentifier).HasColumnName("EmployerGroupIdentifier");
        builder.Property(x => x.JobsApproved).HasColumnName("JobsApproved");
        builder.Property(x => x.JobsApprovedBy).HasColumnName("JobsApprovedBy").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.ImmigrationDisability).HasColumnName("ImmigrationDisability").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.PersonIdentifier).HasColumnName("PersonIdentifier").IsRequired();
        builder.Property(x => x.AccessRevoked).HasColumnName("AccessRevoked");
        builder.Property(x => x.AccessRevokedBy).HasColumnName("AccessRevokedBy").IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.UserAccessGranted).HasColumnName("UserAccessGranted");
        builder.Property(x => x.UserAccessGrantedBy).HasColumnName("UserAccessGrantedBy").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.UserApproveReason).HasColumnName("UserApproveReason").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.ConsentConsultation).HasColumnName("ConsentConsultation").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CandidateCompletionProfilePercent).HasColumnName("CandidateCompletionProfilePercent");
        builder.Property(x => x.CandidateCompletionResumePercent).HasColumnName("CandidateCompletionResumePercent");
        builder.Property(x => x.CandidateIsActivelySeeking).HasColumnName("CandidateIsActivelySeeking");
        builder.Property(x => x.CandidateIsWillingToRelocate).HasColumnName("CandidateIsWillingToRelocate");
        builder.Property(x => x.CandidateLinkedInUrl).HasColumnName("CandidateLinkedInUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.CandidateOccupationList).HasColumnName("CandidateOccupationList").IsUnicode(false);
        builder.Property(x => x.IndustryItemIdentifier).HasColumnName("IndustryItemIdentifier");
        builder.Property(x => x.OccupationStandardIdentifier).HasColumnName("OccupationStandardIdentifier");
        builder.Property(x => x.AccountReviewQueued).HasColumnName("AccountReviewQueued");
        builder.Property(x => x.AccountReviewCompleted).HasColumnName("AccountReviewCompleted");
        builder.Property(x => x.ConsentToShare).HasColumnName("ConsentToShare").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.EmailEnabled).HasColumnName("EmailEnabled").IsRequired();
        builder.Property(x => x.EmailAlternateEnabled).HasColumnName("EmailAlternateEnabled").IsRequired();
        builder.Property(x => x.MembershipStatusItemIdentifier).HasColumnName("MembershipStatusItemIdentifier");
        builder.Property(x => x.FullName).HasColumnName("FullName").IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.LastAuthenticated).HasColumnName("LastAuthenticated");
        builder.Property(x => x.IsOperator).HasColumnName("IsOperator").IsRequired();
        builder.Property(x => x.BillingAddressIdentifier).HasColumnName("BillingAddressIdentifier");
        builder.Property(x => x.HomeAddressIdentifier).HasColumnName("HomeAddressIdentifier");
        builder.Property(x => x.ShippingAddressIdentifier).HasColumnName("ShippingAddressIdentifier");
        builder.Property(x => x.WorkAddressIdentifier).HasColumnName("WorkAddressIdentifier");
        builder.Property(x => x.MarketingEmailEnabled).HasColumnName("MarketingEmailEnabled").IsRequired();
        builder.Property(x => x.IsArchived).HasColumnName("IsArchived").IsRequired();
        builder.Property(x => x.WhenArchived).HasColumnName("WhenArchived");
        builder.Property(x => x.WhenUnarchived).HasColumnName("WhenUnarchived");
        builder.Property(x => x.EmployeeType).HasColumnName("EmployeeType").IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.IsDeveloper).HasColumnName("IsDeveloper").IsRequired();
        builder.Property(x => x.JobDivision).HasColumnName("JobDivision").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.PersonType).HasColumnName("PersonType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SinModified).HasColumnName("SinModified");
        builder.Property(x => x.AgeGroup).HasColumnName("AgeGroup").IsUnicode(false).HasMaxLength(20);
    }
}