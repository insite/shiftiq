using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PersonAdapter : IEntityAdapter
{
    public void Copy(ModifyPerson modify, PersonEntity entity)
    {
        entity.EmployeeUnion = modify.EmployeeUnion;
        entity.IsAdministrator = modify.IsAdministrator;
        entity.SocialInsuranceNumber = modify.SocialInsuranceNumber;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.PersonCode = modify.PersonCode;
        entity.IsLearner = modify.IsLearner;
        entity.Birthdate = modify.Birthdate;
        entity.Citizenship = modify.Citizenship;
        entity.Created = modify.Created;
        entity.CreatedBy = modify.CreatedBy;
        entity.CustomKey = modify.CustomKey;
        entity.EducationLevel = modify.EducationLevel;
        entity.Gender = modify.Gender;
        entity.ImmigrationApplicant = modify.ImmigrationApplicant;
        entity.ImmigrationCategory = modify.ImmigrationCategory;
        entity.ImmigrationDestination = modify.ImmigrationDestination;
        entity.ImmigrationLandingDate = modify.ImmigrationLandingDate;
        entity.ImmigrationNumber = modify.ImmigrationNumber;
        entity.JobTitle = modify.JobTitle;
        entity.Language = modify.Language;
        entity.MemberEndDate = modify.MemberEndDate;
        entity.MemberStartDate = modify.MemberStartDate;
        entity.Modified = modify.Modified;
        entity.ModifiedBy = modify.ModifiedBy;
        entity.Phone = modify.Phone;
        entity.PhoneFax = modify.PhoneFax;
        entity.PhoneHome = modify.PhoneHome;
        entity.PhoneOther = modify.PhoneOther;
        entity.PhoneWork = modify.PhoneWork;
        entity.Referrer = modify.Referrer;
        entity.ReferrerOther = modify.ReferrerOther;
        entity.Region = modify.Region;
        entity.CredentialingCountry = modify.CredentialingCountry;
        entity.ShippingPreference = modify.ShippingPreference;
        entity.TradeworkerNumber = modify.TradeworkerNumber;
        entity.WebSiteUrl = modify.WebSiteUrl;
        entity.WelcomeEmailsSentToUser = modify.WelcomeEmailsSentToUser;
        entity.EmergencyContactName = modify.EmergencyContactName;
        entity.EmergencyContactPhone = modify.EmergencyContactPhone;
        entity.EmergencyContactRelationship = modify.EmergencyContactRelationship;
        entity.FirstLanguage = modify.FirstLanguage;
        entity.EmployerGroupIdentifier = modify.EmployerGroupIdentifier;
        entity.JobsApproved = modify.JobsApproved;
        entity.JobsApprovedBy = modify.JobsApprovedBy;
        entity.ImmigrationDisability = modify.ImmigrationDisability;
        entity.AccessRevoked = modify.AccessRevoked;
        entity.AccessRevokedBy = modify.AccessRevokedBy;
        entity.UserAccessGranted = modify.UserAccessGranted;
        entity.UserAccessGrantedBy = modify.UserAccessGrantedBy;
        entity.UserApproveReason = modify.UserApproveReason;
        entity.ConsentConsultation = modify.ConsentConsultation;
        entity.CandidateCompletionProfilePercent = modify.CandidateCompletionProfilePercent;
        entity.CandidateCompletionResumePercent = modify.CandidateCompletionResumePercent;
        entity.CandidateIsActivelySeeking = modify.CandidateIsActivelySeeking;
        entity.CandidateIsWillingToRelocate = modify.CandidateIsWillingToRelocate;
        entity.CandidateLinkedInUrl = modify.CandidateLinkedInUrl;
        entity.CandidateOccupationList = modify.CandidateOccupationList;
        entity.IndustryItemIdentifier = modify.IndustryItemIdentifier;
        entity.OccupationStandardIdentifier = modify.OccupationStandardIdentifier;
        entity.AccountReviewQueued = modify.AccountReviewQueued;
        entity.AccountReviewCompleted = modify.AccountReviewCompleted;
        entity.ConsentToShare = modify.ConsentToShare;
        entity.EmailEnabled = modify.EmailEnabled;
        entity.EmailAlternateEnabled = modify.EmailAlternateEnabled;
        entity.MembershipStatusItemIdentifier = modify.MembershipStatusItemIdentifier;
        entity.FullName = modify.FullName;
        entity.LastAuthenticated = modify.LastAuthenticated;
        entity.IsOperator = modify.IsOperator;
        entity.BillingAddressIdentifier = modify.BillingAddressIdentifier;
        entity.HomeAddressIdentifier = modify.HomeAddressIdentifier;
        entity.ShippingAddressIdentifier = modify.ShippingAddressIdentifier;
        entity.WorkAddressIdentifier = modify.WorkAddressIdentifier;
        entity.MarketingEmailEnabled = modify.MarketingEmailEnabled;
        entity.IsArchived = modify.IsArchived;
        entity.WhenArchived = modify.WhenArchived;
        entity.WhenUnarchived = modify.WhenUnarchived;
        entity.EmployeeType = modify.EmployeeType;
        entity.IsDeveloper = modify.IsDeveloper;
        entity.JobDivision = modify.JobDivision;
        entity.PersonType = modify.PersonType;
        entity.SinModified = modify.SinModified;
        entity.AgeGroup = modify.AgeGroup;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public PersonEntity ToEntity(CreatePerson create)
    {
        var entity = new PersonEntity
        {
            EmployeeUnion = create.EmployeeUnion,
            IsAdministrator = create.IsAdministrator,
            SocialInsuranceNumber = create.SocialInsuranceNumber,
            OrganizationIdentifier = create.OrganizationIdentifier,
            UserIdentifier = create.UserIdentifier,
            PersonCode = create.PersonCode,
            IsLearner = create.IsLearner,
            Birthdate = create.Birthdate,
            Citizenship = create.Citizenship,
            Created = create.Created,
            CreatedBy = create.CreatedBy,
            CustomKey = create.CustomKey,
            EducationLevel = create.EducationLevel,
            Gender = create.Gender,
            ImmigrationApplicant = create.ImmigrationApplicant,
            ImmigrationCategory = create.ImmigrationCategory,
            ImmigrationDestination = create.ImmigrationDestination,
            ImmigrationLandingDate = create.ImmigrationLandingDate,
            ImmigrationNumber = create.ImmigrationNumber,
            JobTitle = create.JobTitle,
            Language = create.Language,
            MemberEndDate = create.MemberEndDate,
            MemberStartDate = create.MemberStartDate,
            Modified = create.Modified,
            ModifiedBy = create.ModifiedBy,
            Phone = create.Phone,
            PhoneFax = create.PhoneFax,
            PhoneHome = create.PhoneHome,
            PhoneOther = create.PhoneOther,
            PhoneWork = create.PhoneWork,
            Referrer = create.Referrer,
            ReferrerOther = create.ReferrerOther,
            Region = create.Region,
            CredentialingCountry = create.CredentialingCountry,
            ShippingPreference = create.ShippingPreference,
            TradeworkerNumber = create.TradeworkerNumber,
            WebSiteUrl = create.WebSiteUrl,
            WelcomeEmailsSentToUser = create.WelcomeEmailsSentToUser,
            EmergencyContactName = create.EmergencyContactName,
            EmergencyContactPhone = create.EmergencyContactPhone,
            EmergencyContactRelationship = create.EmergencyContactRelationship,
            FirstLanguage = create.FirstLanguage,
            EmployerGroupIdentifier = create.EmployerGroupIdentifier,
            JobsApproved = create.JobsApproved,
            JobsApprovedBy = create.JobsApprovedBy,
            ImmigrationDisability = create.ImmigrationDisability,
            PersonIdentifier = create.PersonIdentifier,
            AccessRevoked = create.AccessRevoked,
            AccessRevokedBy = create.AccessRevokedBy,
            UserAccessGranted = create.UserAccessGranted,
            UserAccessGrantedBy = create.UserAccessGrantedBy,
            UserApproveReason = create.UserApproveReason,
            ConsentConsultation = create.ConsentConsultation,
            CandidateCompletionProfilePercent = create.CandidateCompletionProfilePercent,
            CandidateCompletionResumePercent = create.CandidateCompletionResumePercent,
            CandidateIsActivelySeeking = create.CandidateIsActivelySeeking,
            CandidateIsWillingToRelocate = create.CandidateIsWillingToRelocate,
            CandidateLinkedInUrl = create.CandidateLinkedInUrl,
            CandidateOccupationList = create.CandidateOccupationList,
            IndustryItemIdentifier = create.IndustryItemIdentifier,
            OccupationStandardIdentifier = create.OccupationStandardIdentifier,
            AccountReviewQueued = create.AccountReviewQueued,
            AccountReviewCompleted = create.AccountReviewCompleted,
            ConsentToShare = create.ConsentToShare,
            EmailEnabled = create.EmailEnabled,
            EmailAlternateEnabled = create.EmailAlternateEnabled,
            MembershipStatusItemIdentifier = create.MembershipStatusItemIdentifier,
            FullName = create.FullName,
            LastAuthenticated = create.LastAuthenticated,
            IsOperator = create.IsOperator,
            BillingAddressIdentifier = create.BillingAddressIdentifier,
            HomeAddressIdentifier = create.HomeAddressIdentifier,
            ShippingAddressIdentifier = create.ShippingAddressIdentifier,
            WorkAddressIdentifier = create.WorkAddressIdentifier,
            MarketingEmailEnabled = create.MarketingEmailEnabled,
            IsArchived = create.IsArchived,
            WhenArchived = create.WhenArchived,
            WhenUnarchived = create.WhenUnarchived,
            EmployeeType = create.EmployeeType,
            IsDeveloper = create.IsDeveloper,
            JobDivision = create.JobDivision,
            PersonType = create.PersonType,
            SinModified = create.SinModified,
            AgeGroup = create.AgeGroup
        };
        return entity;
    }

    public IEnumerable<PersonModel> ToModel(IEnumerable<PersonEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PersonModel ToModel(PersonEntity entity)
    {
        var model = new PersonModel
        {
            EmployeeUnion = entity.EmployeeUnion,
            IsAdministrator = entity.IsAdministrator,
            SocialInsuranceNumber = entity.SocialInsuranceNumber,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            UserIdentifier = entity.UserIdentifier,
            PersonCode = entity.PersonCode,
            IsLearner = entity.IsLearner,
            Birthdate = entity.Birthdate,
            Citizenship = entity.Citizenship,
            Created = entity.Created,
            CreatedBy = entity.CreatedBy,
            CustomKey = entity.CustomKey,
            EducationLevel = entity.EducationLevel,
            Gender = entity.Gender,
            ImmigrationApplicant = entity.ImmigrationApplicant,
            ImmigrationCategory = entity.ImmigrationCategory,
            ImmigrationDestination = entity.ImmigrationDestination,
            ImmigrationLandingDate = entity.ImmigrationLandingDate,
            ImmigrationNumber = entity.ImmigrationNumber,
            JobTitle = entity.JobTitle,
            Language = entity.Language,
            MemberEndDate = entity.MemberEndDate,
            MemberStartDate = entity.MemberStartDate,
            Modified = entity.Modified,
            ModifiedBy = entity.ModifiedBy,
            Phone = entity.Phone,
            PhoneFax = entity.PhoneFax,
            PhoneHome = entity.PhoneHome,
            PhoneOther = entity.PhoneOther,
            PhoneWork = entity.PhoneWork,
            Referrer = entity.Referrer,
            ReferrerOther = entity.ReferrerOther,
            Region = entity.Region,
            CredentialingCountry = entity.CredentialingCountry,
            ShippingPreference = entity.ShippingPreference,
            TradeworkerNumber = entity.TradeworkerNumber,
            WebSiteUrl = entity.WebSiteUrl,
            WelcomeEmailsSentToUser = entity.WelcomeEmailsSentToUser,
            EmergencyContactName = entity.EmergencyContactName,
            EmergencyContactPhone = entity.EmergencyContactPhone,
            EmergencyContactRelationship = entity.EmergencyContactRelationship,
            FirstLanguage = entity.FirstLanguage,
            EmployerGroupIdentifier = entity.EmployerGroupIdentifier,
            JobsApproved = entity.JobsApproved,
            JobsApprovedBy = entity.JobsApprovedBy,
            ImmigrationDisability = entity.ImmigrationDisability,
            PersonIdentifier = entity.PersonIdentifier,
            AccessRevoked = entity.AccessRevoked,
            AccessRevokedBy = entity.AccessRevokedBy,
            UserAccessGranted = entity.UserAccessGranted,
            UserAccessGrantedBy = entity.UserAccessGrantedBy,
            UserApproveReason = entity.UserApproveReason,
            ConsentConsultation = entity.ConsentConsultation,
            CandidateCompletionProfilePercent = entity.CandidateCompletionProfilePercent,
            CandidateCompletionResumePercent = entity.CandidateCompletionResumePercent,
            CandidateIsActivelySeeking = entity.CandidateIsActivelySeeking,
            CandidateIsWillingToRelocate = entity.CandidateIsWillingToRelocate,
            CandidateLinkedInUrl = entity.CandidateLinkedInUrl,
            CandidateOccupationList = entity.CandidateOccupationList,
            IndustryItemIdentifier = entity.IndustryItemIdentifier,
            OccupationStandardIdentifier = entity.OccupationStandardIdentifier,
            AccountReviewQueued = entity.AccountReviewQueued,
            AccountReviewCompleted = entity.AccountReviewCompleted,
            ConsentToShare = entity.ConsentToShare,
            EmailEnabled = entity.EmailEnabled,
            EmailAlternateEnabled = entity.EmailAlternateEnabled,
            MembershipStatusItemIdentifier = entity.MembershipStatusItemIdentifier,
            FullName = entity.FullName,
            LastAuthenticated = entity.LastAuthenticated,
            IsOperator = entity.IsOperator,
            BillingAddressIdentifier = entity.BillingAddressIdentifier,
            HomeAddressIdentifier = entity.HomeAddressIdentifier,
            ShippingAddressIdentifier = entity.ShippingAddressIdentifier,
            WorkAddressIdentifier = entity.WorkAddressIdentifier,
            MarketingEmailEnabled = entity.MarketingEmailEnabled,
            IsArchived = entity.IsArchived,
            WhenArchived = entity.WhenArchived,
            WhenUnarchived = entity.WhenUnarchived,
            EmployeeType = entity.EmployeeType,
            IsDeveloper = entity.IsDeveloper,
            JobDivision = entity.JobDivision,
            PersonType = entity.PersonType,
            SinModified = entity.SinModified,
            AgeGroup = entity.AgeGroup
        };

        return model;
    }

    public IEnumerable<PersonMatch> ToMatch(IEnumerable<PersonEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public PersonMatch ToMatch(PersonEntity entity)
    {
        var match = new PersonMatch
        {
            PersonId = entity.PersonIdentifier,
            UserId = entity.UserIdentifier,

            UserEmail = entity.User != null ? entity.User.Email : "-",
            UserName = entity.User != null ? entity.User.FullName : "-",
        };

        return match;
    }
}