using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Contacts
{
    internal static class PersonFieldList
    {
        private static readonly Dictionary<PersonField, IStateFieldMeta> _fields = new Dictionary<PersonField, IStateFieldMeta>
        {
            // Guid
            { PersonField.CreatedBy, new StateFieldMeta { FieldType = StateFieldType.Guid } },
            { PersonField.EmployerGroupIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },
            { PersonField.IndustryItemIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },
            { PersonField.MembershipStatusItemIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },
            { PersonField.OccupationStandardIdentifier, new StateFieldMeta { FieldType = StateFieldType.Guid } },

            // string
            { PersonField.AccessRevokedBy, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { PersonField.AgeGroup, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.CandidateLinkedInUrl, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.CandidateOccupationList, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Citizenship, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ConsentConsultation, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ConsentToShare, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.CredentialingCountry, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EducationLevel, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EmergencyContactName, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EmergencyContactPhone, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EmergencyContactRelationship, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EmployeeType, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.EmployeeUnion, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.FirstLanguage, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.FullName, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Gender, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ImmigrationApplicant, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ImmigrationCategory, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ImmigrationDestination, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ImmigrationDisability, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ImmigrationNumber, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.JobsApprovedBy, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { PersonField.JobDivision, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.JobTitle, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Language, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PersonType, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PersonCode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Phone, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PhoneFax, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PhoneHome, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PhoneOther, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.PhoneWork, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Referrer, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ReferrerOther, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.Region, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.ShippingPreference, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.SocialInsuranceNumber, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.TradeworkerNumber, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.UserAccessGrantedBy, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { PersonField.UserApproveReason, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { PersonField.WebSiteUrl, new StateFieldMeta { FieldType = StateFieldType.Text } },

            // bool
            { PersonField.CandidateIsActivelySeeking, new StateFieldMeta { FieldType = StateFieldType.Bool } },
            { PersonField.CandidateIsWillingToRelocate, new StateFieldMeta { FieldType = StateFieldType.Bool } },
            { PersonField.EmailAlternateEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.EmailEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.IsAdministrator, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.IsArchived, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.IsDeveloper, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.IsLearner, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.IsOperator, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { PersonField.MarketingEmailEnabled, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            
            // int
            { PersonField.CandidateCompletionProfilePercent, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { PersonField.CandidateCompletionResumePercent, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { PersonField.CustomKey, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { PersonField.WelcomeEmailsSentToUser, new StateFieldMeta { FieldType = StateFieldType.Int } },

            // DateTimeOffset
            { PersonField.AccessRevoked, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { PersonField.AccountReviewCompleted, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { PersonField.AccountReviewQueued, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { PersonField.Created, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { PersonField.JobsApproved, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { PersonField.LastAuthenticated, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { PersonField.UserAccessGranted, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { PersonField.WhenArchived, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { PersonField.WhenUnarchived, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { PersonField.SinModified, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },

            // DateTime
            { PersonField.Birthdate, new StateFieldMeta { FieldType = StateFieldType.Date } },
            { PersonField.ImmigrationLandingDate, new StateFieldMeta { FieldType = StateFieldType.Date } },
            { PersonField.MemberEndDate, new StateFieldMeta { FieldType = StateFieldType.Date } },
            { PersonField.MemberStartDate, new StateFieldMeta { FieldType = StateFieldType.Date } },
        };

        public static IStateFieldMeta GetField(PersonField userField) => _fields[userField];

        public static IReadOnlyDictionary<PersonField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<PersonField, IStateFieldMeta>(_fields);
    }
}
