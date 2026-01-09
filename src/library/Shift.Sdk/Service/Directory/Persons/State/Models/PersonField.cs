using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InSite.Domain.Contacts
{
    [JsonConverter(typeof(PersonFieldJsonConverter))]
    public enum PersonField
    {
        // Guid
        CreatedBy,
        EmployerGroupIdentifier,
        IndustryItemIdentifier,
        MembershipStatusItemIdentifier,
        OccupationStandardIdentifier,

        // string
        AccessRevokedBy,
        CandidateLinkedInUrl,
        CandidateOccupationList,
        Citizenship,
        ConsentConsultation,
        ConsentToShare,
        CredentialingCountry,
        EducationLevel,
        EmergencyContactName,
        EmergencyContactPhone,
        EmergencyContactRelationship,
        EmployeeUnion,
        FirstLanguage,
        FullName,
        Gender,
        ImmigrationApplicant,
        ImmigrationCategory,
        ImmigrationDestination,
        ImmigrationDisability,
        ImmigrationNumber,
        JobsApprovedBy,
        JobTitle,
        Language,
        PersonCode,
        Phone,
        PhoneFax,
        PhoneHome,
        PhoneOther,
        PhoneWork,
        Referrer,
        ReferrerOther,
        Region,
        ShippingPreference,
        SocialInsuranceNumber,
        TradeworkerNumber,
        UserAccessGrantedBy,
        UserApproveReason,
        WebSiteUrl,

        // bool
        CandidateIsActivelySeeking,
        CandidateIsWillingToRelocate,
        EmailAlternateEnabled,
        EmailEnabled,
        IsAdministrator,
        IsLearner,
        IsOperator,
        MarketingEmailEnabled,

        // int
        CandidateCompletionProfilePercent,
        CandidateCompletionResumePercent,
        CustomKey,
        WelcomeEmailsSentToUser,

        // DateTimeOffset
        AccessRevoked,
        AccountReviewCompleted,
        AccountReviewQueued,
        Created,
        JobsApproved,
        LastAuthenticated,
        UserAccessGranted,
        SinModified,

        // DateTime
        Birthdate,
        ImmigrationLandingDate,
        MemberEndDate,
        MemberStartDate,

        // New!
        IsArchived,
        WhenArchived,
        WhenUnarchived,
        IsDeveloper,
        JobDivision,
        PersonType,
        EmployeeType,
        AgeGroup
    }

    public static class PersonFieldHelper
    {
        public const int ObsoleteFieldValue = -4170698;

        public static bool IsObsolete(this PersonField field) => (int)field == ObsoleteFieldValue;
    }

    internal class PersonFieldJsonConverter : StringEnumConverter
    {
        private static readonly HashSet<string> _obsoleteFields = new HashSet<string>
        {
            "EthereumAddress",
            "EthereumNonce",
            "EthereumNonceSignature",
            "AccountStatusName",
            "MembershipStatusItemKey"

        };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value?.ToString();
                if (_obsoleteFields.Contains(value))
                    return PersonFieldHelper.ObsoleteFieldValue;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}
