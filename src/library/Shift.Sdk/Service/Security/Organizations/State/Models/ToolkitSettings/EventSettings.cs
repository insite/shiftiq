using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class EventSettings
    {
        public bool AllowLoginAnyTime { get; set; }
        public bool AllowUserAccountCreationDuringRegistration { get; set; }
        public bool AllowUsersRegisterEmployees { get; set; }
        public bool HideReturnToCalendar { get; set; }
        public bool CompanySelectionAndCreationDisabledDuringRegistration { get; set; }
        public bool ShowUnapplicableSeats { get; set; }
        public bool AllowClassRegistrationFields { get; set; }
        public RegisterEmployeesSearchRequirement RegisterEmployeesSearchRequirement { get; set; }

        [DefaultValue(24), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int OnlineEventAutomationWindowHours { get; set; } = 24;

        [DefaultValue(4), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int PaperEventAutomationWindowMonths { get; set; } = 4;

        public bool IsEqual(EventSettings other)
        {
            return AllowLoginAnyTime == other.AllowLoginAnyTime
                && AllowUserAccountCreationDuringRegistration == other.AllowUserAccountCreationDuringRegistration
                && AllowUsersRegisterEmployees == other.AllowUsersRegisterEmployees
                && HideReturnToCalendar == other.HideReturnToCalendar
                && CompanySelectionAndCreationDisabledDuringRegistration == other.CompanySelectionAndCreationDisabledDuringRegistration
                && ShowUnapplicableSeats == other.ShowUnapplicableSeats
                && AllowClassRegistrationFields == other.AllowClassRegistrationFields
                && RegisterEmployeesSearchRequirement == other.RegisterEmployeesSearchRequirement
                && OnlineEventAutomationWindowHours == other.OnlineEventAutomationWindowHours
                && PaperEventAutomationWindowMonths == other.PaperEventAutomationWindowMonths;
        }

        public void Set(EventSettings value)
        {
            AllowLoginAnyTime = value.AllowLoginAnyTime;
            AllowUserAccountCreationDuringRegistration = value.AllowUserAccountCreationDuringRegistration;
            AllowUsersRegisterEmployees = value.AllowUsersRegisterEmployees;
            HideReturnToCalendar = value.HideReturnToCalendar;
            CompanySelectionAndCreationDisabledDuringRegistration = value.CompanySelectionAndCreationDisabledDuringRegistration;
            ShowUnapplicableSeats = value.ShowUnapplicableSeats;
            AllowClassRegistrationFields = value.AllowClassRegistrationFields;
            RegisterEmployeesSearchRequirement = value.RegisterEmployeesSearchRequirement;
            OnlineEventAutomationWindowHours = value.OnlineEventAutomationWindowHours;
            PaperEventAutomationWindowMonths = value.PaperEventAutomationWindowMonths;
        }
    }

    public enum RegisterEmployeesSearchRequirement
    {
        NameAndEmail,       // (UserLastName AND Email) = Default for all organizations
        NameAndEmailOrCode, // (UserLastName AND (UserEmail OR PersonCode) = Default for RCABC
        NameOrEmail,        // (UserLastName OR Email)
        Email               // Email = Default for Keyera
    }
}
