using System;
using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Application.Events.Read;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Registrations.Read;

namespace InSite.Application.Contacts.Read
{
    public class VPerson
    {
        public Guid? BillingAddressIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid? HomeAddressIdentifier { get; set; }
        public Guid? ShippingAddressIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? WorkAddressIdentifier { get; set; }
        public Guid? EmployerGroupStatusItemIdentifier { get; set; }

        public string EmployerGroupName { get; set; }
        public string EmployerGroupStatus { get; set; }
        public string EmployerGroupRegion { get; set; }
        public string PersonCode { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string Region { get; set; }
        public string TradeworkerNumber { get; set; }
        public string UserEmail { get; set; }
        public string UserEmailAlternate { get; set; }
        public string UserFirstName { get; set; }
        public string UserFullName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }
        public string UserTimeZone { get; set; }
        public bool? CandidateIsActivelySeeking { get; set; }
        public bool IsArchived { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsLearner { get; set; }
        public bool UserEmailEnabled { get; set; }
        public bool UserEmailAlternateEnabled { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTimeOffset? UtcArchived { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string Language { get; set; }
        public string FirstLanguage { get; set; }

        public virtual VUser User { get; set; }
        public virtual VAddress BillingAddress { get; set; }
        public virtual VAddress HomeAddress { get; set; }
        public virtual VAddress ShippingAddress { get; set; }
        public virtual VAddress WorkAddress { get; set; }

        public virtual ICollection<QAttempt> AssessorAttempts { get; set; }
        public virtual ICollection<QAttempt> LearnerAttempts { get; set; }
        public virtual ICollection<QEvent> VenueCoordinatorEvents { get; set; }
        public virtual ICollection<QEventAttendee> EventAttendees { get; set; }
        public virtual ICollection<QRegistration> Registrations { get; set; }
        public virtual ICollection<QRegistration> RequestedRegistrations { get; set; }
        public virtual ICollection<QRegistrationInstructor> RegistrationInstructors { get; set; }
        public virtual ICollection<TQuizAttempt> LearnerQuizAttempts { get; set; }

        public VPerson()
        {
            AssessorAttempts = new HashSet<QAttempt>();
            LearnerAttempts = new HashSet<QAttempt>();
            VenueCoordinatorEvents = new HashSet<QEvent>();
            EventAttendees = new HashSet<QEventAttendee>();
            Registrations = new HashSet<QRegistration>();
            RequestedRegistrations = new HashSet<QRegistration>();
            RegistrationInstructors = new HashSet<QRegistrationInstructor>();
            LearnerQuizAttempts = new HashSet<TQuizAttempt>();
        }
    }
}
