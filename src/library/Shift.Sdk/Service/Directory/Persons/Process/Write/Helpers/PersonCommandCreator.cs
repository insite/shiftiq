using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Write
{
    public class PersonCommandCreator
    {
        private readonly QPerson _old;
        private readonly QPerson _new;

        private List<Command> _result = new List<Command>();

        private PersonCommandCreator(QPerson oldPerson, QPerson newPerson)
        {
            _old = oldPerson;
            _new = newPerson;
        }

        public static List<Command> Create(QPerson oldPerson, QPerson newPerson)
        {
            return new PersonCommandCreator(oldPerson, newPerson).Create();
        }

        private List<Command> Create()
        {
            AddCreateCommand();
            AddApproveJobCommand();
            AddAccessCommand();
            AddOtherTextChanges();
            AddOtherDateChanges();
            AddOtherBoolChanges();
            AddOtherIntChanges();
            AddOtherGuidChanges();

            AddAddressCommand(AddressType.Billing, _new.BillingAddress);
            AddAddressCommand(AddressType.Shipping, _new.ShippingAddress);
            AddAddressCommand(AddressType.Work, _new.WorkAddress);
            AddAddressCommand(AddressType.Home, _new.HomeAddress);

            return _result;
        }

        private void AddAddressCommand(AddressType addressType, QPersonAddress addressEntity)
        {
            if (addressEntity == null)
                return;

            PersonAddress address;

            if (string.IsNullOrWhiteSpace(addressEntity.City)
                && string.IsNullOrWhiteSpace(addressEntity.Country)
                && string.IsNullOrWhiteSpace(addressEntity.Description)
                && string.IsNullOrWhiteSpace(addressEntity.PostalCode)
                && string.IsNullOrWhiteSpace(addressEntity.Province)
                && string.IsNullOrWhiteSpace(addressEntity.Street1)
                && string.IsNullOrWhiteSpace(addressEntity.Street2)
                )
            {
                if (addressEntity.AddressIdentifier == Guid.Empty)
                    return;

                address = null;
            }
            else
            {
                address = new PersonAddress
                {
                    Identifier = addressEntity.AddressIdentifier != Guid.Empty ? addressEntity.AddressIdentifier : UuidFactory.Create(),
                    City = addressEntity.City,
                    Country = addressEntity.Country,
                    Description = addressEntity.Description,
                    PostalCode = addressEntity.PostalCode,
                    Province = addressEntity.Province,
                    Street1 = addressEntity.Street1,
                    Street2 = addressEntity.Street2,
                };
            }

            _result.Add(new ModifyPersonAddress(_new.PersonIdentifier, addressType, address));
        }

        private void AddCreateCommand()
        {
            if (_old == null)
                _result.Add(new CreatePerson(_new.PersonIdentifier, _new.UserIdentifier, _new.OrganizationIdentifier));
        }

        private void AddApproveJobCommand()
        {
            if (_old?.JobsApproved != _new.JobsApproved || !StringHelper.EqualsCaseSensitive(_old?.JobsApprovedBy, _new.JobsApprovedBy, true))
                _result.Add(new ApprovePersonJob(_new.PersonIdentifier, _new.JobsApproved, _new.JobsApprovedBy));
        }

        private void AddAccessCommand()
        {
            if (_old?.UserAccessGranted == _new.UserAccessGranted
                && StringHelper.EqualsCaseSensitive(_old?.UserAccessGrantedBy, _new.UserAccessGrantedBy, true)
                && _old?.AccessRevoked == _new.AccessRevoked
                && StringHelper.EqualsCaseSensitive(_old?.AccessRevokedBy, _new.AccessRevokedBy, true)
                )
            {
                return;
            }

            if (_new.UserAccessGranted.HasValue)
                _result.Add(new GrantPersonAccess(_new.PersonIdentifier, _new.UserAccessGranted.Value, _new.UserAccessGrantedBy));
            else if (_new.AccessRevoked.HasValue)
                _result.Add(new RevokePersonAccess(_new.PersonIdentifier, _new.AccessRevoked.Value, _new.AccessRevokedBy));
        }

        private void AddOtherTextChanges()
        {

            if (!StringHelper.EqualsCaseSensitive(_old?.AgeGroup, _new.AgeGroup, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.AgeGroup, _new.AgeGroup));

            if (!StringHelper.EqualsCaseSensitive(_old?.CandidateLinkedInUrl, _new.CandidateLinkedInUrl, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.CandidateLinkedInUrl, _new.CandidateLinkedInUrl));

            if (!StringHelper.EqualsCaseSensitive(_old?.CandidateOccupationList, _new.CandidateOccupationList, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.CandidateOccupationList, _new.CandidateOccupationList));

            if (!StringHelper.EqualsCaseSensitive(_old?.Citizenship, _new.Citizenship, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Citizenship, _new.Citizenship));

            if (!StringHelper.EqualsCaseSensitive(_old?.ConsentConsultation, _new.ConsentConsultation, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ConsentConsultation, _new.ConsentConsultation));

            if (!StringHelper.EqualsCaseSensitive(_old?.ConsentToShare, _new.ConsentToShare, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ConsentToShare, _new.ConsentToShare));

            if (!StringHelper.EqualsCaseSensitive(_old?.CredentialingCountry, _new.CredentialingCountry, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.CredentialingCountry, _new.CredentialingCountry));

            if (!StringHelper.EqualsCaseSensitive(_old?.EducationLevel, _new.EducationLevel, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EducationLevel, _new.EducationLevel));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmergencyContactName, _new.EmergencyContactName, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EmergencyContactName, _new.EmergencyContactName));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmergencyContactPhone, _new.EmergencyContactPhone, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EmergencyContactPhone, _new.EmergencyContactPhone));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmergencyContactRelationship, _new.EmergencyContactRelationship, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EmergencyContactRelationship, _new.EmergencyContactRelationship));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmployeeType, _new.EmployeeType, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EmployeeType, _new.EmployeeType));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmployeeUnion, _new.EmployeeUnion, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.EmployeeUnion, _new.EmployeeUnion));

            if (!StringHelper.EqualsCaseSensitive(_old?.FirstLanguage, _new.FirstLanguage, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.FirstLanguage, _new.FirstLanguage));

            if (!StringHelper.EqualsCaseSensitive(_old?.Gender, _new.Gender, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Gender, _new.Gender));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImmigrationApplicant, _new.ImmigrationApplicant, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ImmigrationApplicant, _new.ImmigrationApplicant));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImmigrationCategory, _new.ImmigrationCategory, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ImmigrationCategory, _new.ImmigrationCategory));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImmigrationDestination, _new.ImmigrationDestination, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ImmigrationDestination, _new.ImmigrationDestination));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImmigrationDisability, _new.ImmigrationDisability, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ImmigrationDisability, _new.ImmigrationDisability));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImmigrationNumber, _new.ImmigrationNumber, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ImmigrationNumber, _new.ImmigrationNumber));

            if (!StringHelper.EqualsCaseSensitive(_old?.JobDivision, _new.JobDivision, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.JobDivision, _new.JobDivision));

            if (!StringHelper.EqualsCaseSensitive(_old?.JobTitle, _new.JobTitle, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.JobTitle, _new.JobTitle));

            if (!StringHelper.EqualsCaseSensitive(_old?.Language, _new.Language, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Language, _new.Language));

            if (!StringHelper.EqualsCaseSensitive(_old?.PersonCode, _new.PersonCode, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PersonCode, _new.PersonCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.PersonType, _new.PersonType, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PersonType, _new.PersonType));

            if (!StringHelper.EqualsCaseSensitive(_old?.Phone, _new.Phone, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Phone, _new.Phone));

            if (!StringHelper.EqualsCaseSensitive(_old?.PhoneFax, _new.PhoneFax, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PhoneFax, _new.PhoneFax));

            if (!StringHelper.EqualsCaseSensitive(_old?.PhoneHome, _new.PhoneHome, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PhoneHome, _new.PhoneHome));

            if (!StringHelper.EqualsCaseSensitive(_old?.PhoneOther, _new.PhoneOther, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PhoneOther, _new.PhoneOther));

            if (!StringHelper.EqualsCaseSensitive(_old?.PhoneWork, _new.PhoneWork, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.PhoneWork, _new.PhoneWork));

            if (!StringHelper.EqualsCaseSensitive(_old?.Referrer, _new.Referrer, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Referrer, _new.Referrer));

            if (!StringHelper.EqualsCaseSensitive(_old?.ReferrerOther, _new.ReferrerOther, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ReferrerOther, _new.ReferrerOther));

            if (!StringHelper.EqualsCaseSensitive(_old?.Region, _new.Region, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.Region, _new.Region));

            if (!StringHelper.EqualsCaseSensitive(_old?.ShippingPreference, _new.ShippingPreference, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.ShippingPreference, _new.ShippingPreference));

            if (!StringHelper.EqualsCaseSensitive(_old?.SocialInsuranceNumber, _new.SocialInsuranceNumber, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.SocialInsuranceNumber, _new.SocialInsuranceNumber));

            if (!StringHelper.EqualsCaseSensitive(_old?.TradeworkerNumber, _new.TradeworkerNumber, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.TradeworkerNumber, _new.TradeworkerNumber));

            if (!StringHelper.EqualsCaseSensitive(_old?.UserApproveReason, _new.UserApproveReason, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.UserApproveReason, _new.UserApproveReason));

            if (!StringHelper.EqualsCaseSensitive(_old?.WebSiteUrl, _new.WebSiteUrl, true))
                _result.Add(new ModifyPersonFieldText(_new.PersonIdentifier, PersonField.WebSiteUrl, _new.WebSiteUrl));
        }

        private void AddOtherDateChanges()
        {
            if (_old?.AccountReviewCompleted != _new.AccountReviewCompleted)
                _result.Add(new ModifyPersonFieldDateOffset(_new.PersonIdentifier, PersonField.AccountReviewCompleted, _new.AccountReviewCompleted));

            if (_old?.AccountReviewQueued != _new.AccountReviewQueued)
                _result.Add(new ModifyPersonFieldDateOffset(_new.PersonIdentifier, PersonField.AccountReviewQueued, _new.AccountReviewQueued));

            if (_old?.LastAuthenticated != _new.LastAuthenticated)
                _result.Add(new ModifyPersonFieldDateOffset(_new.PersonIdentifier, PersonField.LastAuthenticated, _new.LastAuthenticated));

            if (_old?.Birthdate != _new.Birthdate)
                _result.Add(new ModifyPersonFieldDate(_new.PersonIdentifier, PersonField.Birthdate, _new.Birthdate));

            if (_old?.ImmigrationLandingDate != _new.ImmigrationLandingDate)
                _result.Add(new ModifyPersonFieldDate(_new.PersonIdentifier, PersonField.ImmigrationLandingDate, _new.ImmigrationLandingDate));

            if (_old?.MemberEndDate != _new.MemberEndDate)
                _result.Add(new ModifyPersonFieldDate(_new.PersonIdentifier, PersonField.MemberEndDate, _new.MemberEndDate));

            if (_old?.MemberStartDate != _new.MemberStartDate)
                _result.Add(new ModifyPersonFieldDate(_new.PersonIdentifier, PersonField.MemberStartDate, _new.MemberStartDate));
        }

        private void AddOtherBoolChanges()
        {
            if (_old?.CandidateIsActivelySeeking != _new.CandidateIsActivelySeeking)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.CandidateIsActivelySeeking, _new.CandidateIsActivelySeeking));

            if (_old?.CandidateIsWillingToRelocate != _new.CandidateIsWillingToRelocate)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.CandidateIsWillingToRelocate, _new.CandidateIsWillingToRelocate));

            if (_old?.EmailAlternateEnabled != _new.EmailAlternateEnabled)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.EmailAlternateEnabled, _new.EmailAlternateEnabled));

            if (_old?.EmailEnabled != _new.EmailEnabled)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.EmailEnabled, _new.EmailEnabled));

            if (_old?.IsAdministrator != _new.IsAdministrator)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.IsAdministrator, _new.IsAdministrator));

            if (_old?.IsDeveloper != _new.IsDeveloper)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.IsDeveloper, _new.IsDeveloper));

            if (_old?.IsLearner != _new.IsLearner)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.IsLearner, _new.IsLearner));

            if (_old?.IsOperator != _new.IsOperator)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.IsOperator, _new.IsOperator));

            if (_old?.MarketingEmailEnabled != _new.MarketingEmailEnabled)
                _result.Add(new ModifyPersonFieldBool(_new.PersonIdentifier, PersonField.MarketingEmailEnabled, _new.MarketingEmailEnabled));
        }

        private void AddOtherIntChanges()
        {
            if (_old?.CandidateCompletionProfilePercent != _new.CandidateCompletionProfilePercent)
                _result.Add(new ModifyPersonFieldInt(_new.PersonIdentifier, PersonField.CandidateCompletionProfilePercent, _new.CandidateCompletionProfilePercent));

            if (_old?.CandidateCompletionResumePercent != _new.CandidateCompletionResumePercent)
                _result.Add(new ModifyPersonFieldInt(_new.PersonIdentifier, PersonField.CandidateCompletionResumePercent, _new.CandidateCompletionResumePercent));

            if (_old?.CustomKey != _new.CustomKey)
                _result.Add(new ModifyPersonFieldInt(_new.PersonIdentifier, PersonField.CustomKey, _new.CustomKey));


            if (_old?.WelcomeEmailsSentToUser != _new.WelcomeEmailsSentToUser)
                _result.Add(new ModifyPersonFieldInt(_new.PersonIdentifier, PersonField.WelcomeEmailsSentToUser, _new.WelcomeEmailsSentToUser));
        }

        private void AddOtherGuidChanges()
        {
            if (_old?.EmployerGroupIdentifier != _new.EmployerGroupIdentifier)
                _result.Add(new ModifyPersonFieldGuid(_new.PersonIdentifier, PersonField.EmployerGroupIdentifier, _new.EmployerGroupIdentifier));

            if (_old?.IndustryItemIdentifier != _new.IndustryItemIdentifier)
                _result.Add(new ModifyPersonFieldGuid(_new.PersonIdentifier, PersonField.IndustryItemIdentifier, _new.IndustryItemIdentifier));

            if (_old?.MembershipStatusItemIdentifier != _new.MembershipStatusItemIdentifier)
                _result.Add(new ModifyPersonFieldGuid(_new.PersonIdentifier, PersonField.MembershipStatusItemIdentifier, _new.MembershipStatusItemIdentifier));

            if (_old?.OccupationStandardIdentifier != _new.OccupationStandardIdentifier)
                _result.Add(new ModifyPersonFieldGuid(_new.PersonIdentifier, PersonField.OccupationStandardIdentifier, _new.OccupationStandardIdentifier));
        }
    }
}
