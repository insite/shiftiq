using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the projector for Person changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class PersonChangeProjector
    {
        private readonly IPersonStore _store;

        public PersonChangeProjector(IChangeQueue publisher, IPersonStore store)
        {
            _store = store;

            publisher.Subscribe<PersonCreated>(Handle);
            publisher.Subscribe<PersonArchived>(Handle);
            publisher.Subscribe<PersonUnarchived>(Handle);
            publisher.Subscribe<PersonDeleted>(Handle);
            publisher.Subscribe<PersonAddressModified>(Handle);
            publisher.Subscribe<PersonCommentModified>(Handle);
            publisher.Subscribe<PersonJobApproved>(Handle);
            publisher.Subscribe<PersonAccessGranted>(Handle);
            publisher.Subscribe<PersonAccessRevoked>(Handle);
            publisher.Subscribe<PersonFieldTextModified>(Handle);
            publisher.Subscribe<PersonFieldDateOffsetFixed>(Handle);
            publisher.Subscribe<PersonFieldDateOffsetModified>(Handle);
            publisher.Subscribe<PersonFieldDateModified>(Handle);
            publisher.Subscribe<PersonFieldBoolModified>(Handle);
            publisher.Subscribe<PersonFieldIntModified>(Handle);
            publisher.Subscribe<PersonFieldGuidModified>(Handle);
        }

        public void Handle(PersonCreated e)
            => _store.Insert(e);

        public void Handle(PersonArchived e)
            => _store.Update(e, x =>
            {
                x.IsArchived = true;
                x.WhenArchived = e.Date;
                x.WhenUnarchived = null;
            });

        public void Handle(PersonUnarchived e)
            => _store.Update(e, x =>
            {
                x.IsArchived = false;
                x.WhenArchived = null;
                x.WhenUnarchived = e.Date;
            });

        public void Handle(PersonDeleted e)
            => _store.Delete(e);

        public void Handle(PersonAddressModified e)
            => _store.Update(e);

        public void Handle(PersonCommentModified e)
            => _store.Update(e);

        public void Handle(PersonJobApproved e)
            => _store.Update(e, x =>
            {
                x.JobsApproved = e.Approved;
                x.JobsApprovedBy = e.ApprovedBy;
            });

        public void Handle(PersonAccessGranted e)
            => _store.Update(e, x =>
            {
                x.UserAccessGranted = e.Granted;
                x.UserAccessGrantedBy = e.GrantedBy;
                x.AccessRevoked = null;
                x.AccessRevokedBy = null;
            });

        public void Handle(PersonAccessRevoked e)
            => _store.Update(e, x =>
            {
                x.AccessRevoked = e.Revoked;
                x.AccessRevokedBy = e.RevokedBy;
                x.UserAccessGranted = null;
                x.UserAccessGrantedBy = null;
            });

        public void Handle(PersonFieldTextModified e)
        {
            if (e.PersonField.IsObsolete())
                return;

            var state = (PersonState)e.AggregateState;

            _store.Update(e, x =>
            {
                switch (e.PersonField)
                {
                    case PersonField.AgeGroup:
                        x.AgeGroup = e.Value;
                        break;
                    case PersonField.CandidateLinkedInUrl:
                        x.CandidateLinkedInUrl = e.Value;
                        break;
                    case PersonField.CandidateOccupationList:
                        x.CandidateOccupationList = e.Value;
                        break;
                    case PersonField.Citizenship:
                        x.Citizenship = e.Value;
                        break;
                    case PersonField.ConsentConsultation:
                        x.ConsentConsultation = e.Value;
                        break;
                    case PersonField.ConsentToShare:
                        x.ConsentToShare = e.Value;
                        break;
                    case PersonField.CredentialingCountry:
                        x.CredentialingCountry = e.Value;
                        break;
                    case PersonField.EducationLevel:
                        x.EducationLevel = e.Value;
                        break;
                    case PersonField.EmergencyContactName:
                        x.EmergencyContactName = e.Value;
                        break;
                    case PersonField.EmergencyContactPhone:
                        x.EmergencyContactPhone = e.Value;
                        break;
                    case PersonField.EmergencyContactRelationship:
                        x.EmergencyContactRelationship = e.Value;
                        break;
                    case PersonField.EmployeeType:
                        x.EmployeeType = e.Value;
                        break;
                    case PersonField.EmployeeUnion:
                        x.EmployeeUnion = e.Value;
                        break;
                    case PersonField.FirstLanguage:
                        x.FirstLanguage = e.Value;
                        break;
                    case PersonField.FullName:
                        x.FullName = e.Value;
                        break;
                    case PersonField.Gender:
                        x.Gender = e.Value;
                        break;
                    case PersonField.ImmigrationApplicant:
                        x.ImmigrationApplicant = e.Value;
                        break;
                    case PersonField.ImmigrationCategory:
                        x.ImmigrationCategory = e.Value;
                        break;
                    case PersonField.ImmigrationDestination:
                        x.ImmigrationDestination = e.Value;
                        break;
                    case PersonField.ImmigrationDisability:
                        x.ImmigrationDisability = e.Value;
                        break;
                    case PersonField.ImmigrationNumber:
                        x.ImmigrationNumber = e.Value;
                        break;
                    case PersonField.JobDivision:
                        x.JobDivision = e.Value;
                        break;
                    case PersonField.JobTitle:
                        x.JobTitle = e.Value;
                        break;
                    case PersonField.Language:
                        x.Language = e.Value;
                        break;
                    case PersonField.PersonCode:
                        x.PersonCode = e.Value;
                        break;
                    case PersonField.PersonType:
                        x.PersonType = e.Value;
                        break;
                    case PersonField.Phone:
                        x.Phone = e.Value;
                        break;
                    case PersonField.PhoneFax:
                        x.PhoneFax = e.Value;
                        break;
                    case PersonField.PhoneHome:
                        x.PhoneHome = e.Value;
                        break;
                    case PersonField.PhoneOther:
                        x.PhoneOther = e.Value;
                        break;
                    case PersonField.PhoneWork:
                        x.PhoneWork = e.Value;
                        break;
                    case PersonField.Referrer:
                        x.Referrer = e.Value;
                        break;
                    case PersonField.ReferrerOther:
                        x.ReferrerOther = e.Value;
                        break;
                    case PersonField.Region:
                        x.Region = e.Value;
                        break;
                    case PersonField.ShippingPreference:
                        x.ShippingPreference = e.Value;
                        break;
                    case PersonField.SocialInsuranceNumber:
                        x.SocialInsuranceNumber = e.Value;
                        x.SinModified = state.GetDateOffsetValue(PersonField.SinModified);
                        break;
                    case PersonField.TradeworkerNumber:
                        x.TradeworkerNumber = e.Value;
                        break;
                    case PersonField.UserApproveReason:
                        x.UserApproveReason = e.Value;
                        break;
                    case PersonField.WebSiteUrl:
                        x.WebSiteUrl = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported text person field: {e.PersonField}");
                }
            });
        }

        public void Handle(PersonFieldDateOffsetFixed e) => UpdateDateOffsetField(e, e.PersonField, e.Value);

        public void Handle(PersonFieldDateOffsetModified e) => UpdateDateOffsetField(e, e.PersonField, e.Value);

        private void UpdateDateOffsetField(Change e, PersonField field, DateTimeOffset? value)
        {
            if (field.IsObsolete())
                return;

            _store.Update(e, x =>
            {
                switch (field)
                {
                    case PersonField.AccountReviewCompleted:
                        x.AccountReviewCompleted = value;
                        break;
                    case PersonField.AccountReviewQueued:
                        x.AccountReviewQueued = value;
                        break;
                    case PersonField.Created:
                        x.Created = value ?? e.ChangeTime;
                        break;
                    case PersonField.LastAuthenticated:
                        x.LastAuthenticated = value;
                        break;
                    case PersonField.WhenArchived:
                        x.WhenArchived = value;
                        break;
                    case PersonField.WhenUnarchived:
                        x.WhenUnarchived = value;
                        break;
                    case PersonField.SinModified:
                        x.SinModified = value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported date offset person field: {field}");
                }
            });
        }

        public void Handle(PersonFieldDateModified e)
        {
            if (e.PersonField.IsObsolete())
                return;

            _store.Update(e, x =>
            {
                switch (e.PersonField)
                {
                    case PersonField.Birthdate:
                        x.Birthdate = e.Value;
                        break;
                    case PersonField.ImmigrationLandingDate:
                        x.ImmigrationLandingDate = e.Value;
                        break;
                    case PersonField.MemberEndDate:
                        x.MemberEndDate = e.Value;
                        break;
                    case PersonField.MemberStartDate:
                        x.MemberStartDate = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported date person field: {e.PersonField}");
                }
            });
        }

        public void Handle(PersonFieldBoolModified e)
        {
            if (e.PersonField.IsObsolete())
                return;

            _store.Update(e, x =>
            {
                switch (e.PersonField)
                {
                    case PersonField.CandidateIsActivelySeeking:
                        x.CandidateIsActivelySeeking = e.Value;
                        break;
                    case PersonField.CandidateIsWillingToRelocate:
                        x.CandidateIsWillingToRelocate = e.Value;
                        break;
                    case PersonField.EmailAlternateEnabled:
                        x.EmailAlternateEnabled = e.Value ?? throw new ArgumentNullException("EmailAlternateEnabled");
                        break;
                    case PersonField.EmailEnabled:
                        x.EmailEnabled = e.Value ?? throw new ArgumentNullException("EmailEnabled");
                        break;
                    case PersonField.IsAdministrator:
                        x.IsAdministrator = e.Value ?? throw new ArgumentNullException("IsAdministrator");
                        break;
                    case PersonField.IsDeveloper:
                        x.IsDeveloper = e.Value ?? throw new ArgumentNullException("IsDeveloper");
                        break;
                    case PersonField.IsLearner:
                        x.IsLearner = e.Value ?? throw new ArgumentNullException("IsLearner");
                        break;
                    case PersonField.IsOperator:
                        x.IsOperator = e.Value ?? throw new ArgumentNullException("IsOperator");
                        break;
                    case PersonField.MarketingEmailEnabled:
                        x.MarketingEmailEnabled = e.Value ?? throw new ArgumentNullException("MarketingEmailEnabled");
                        break;
                    case PersonField.IsArchived:
                        x.IsArchived = e.Value ?? false;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported bool person field: {e.PersonField}");
                }
            });
        }

        public void Handle(PersonFieldIntModified e)
        {
            if (e.PersonField.IsObsolete())
                return;

            _store.Update(e, x =>
            {
                switch (e.PersonField)
                {
                    case PersonField.CandidateCompletionProfilePercent:
                        x.CandidateCompletionProfilePercent = e.Value;
                        break;
                    case PersonField.CandidateCompletionResumePercent:
                        x.CandidateCompletionResumePercent = e.Value;
                        break;
                    case PersonField.CustomKey:
                        x.CustomKey = e.Value;
                        break;
                    case PersonField.WelcomeEmailsSentToUser:
                        x.WelcomeEmailsSentToUser = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported integer person field: {e.PersonField}");
                }
            });
        }

        public void Handle(PersonFieldGuidModified e)
        {
            _store.Update(e, x =>
            {
                switch (e.PersonField)
                {
                    case PersonField.CreatedBy:
                        x.CreatedBy = e.Value ?? UserIdentifiers.Someone;
                        break;
                    case PersonField.EmployerGroupIdentifier:
                        x.EmployerGroupIdentifier = e.Value;
                        break;
                    case PersonField.IndustryItemIdentifier:
                        x.IndustryItemIdentifier = e.Value;
                        break;
                    case PersonField.MembershipStatusItemIdentifier:
                        x.MembershipStatusItemIdentifier = e.Value;
                        break;
                    case PersonField.OccupationStandardIdentifier:
                        x.OccupationStandardIdentifier = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported GUID person field: {e.PersonField}");
                }
            });
        }
    }
}
