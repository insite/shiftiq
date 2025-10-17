using System;

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

        public bool IsEqual(EventSettings other)
        {
            return AllowLoginAnyTime == other.AllowLoginAnyTime
                && AllowUserAccountCreationDuringRegistration == other.AllowUserAccountCreationDuringRegistration
                && AllowUsersRegisterEmployees == other.AllowUsersRegisterEmployees
                && HideReturnToCalendar == other.HideReturnToCalendar
                && CompanySelectionAndCreationDisabledDuringRegistration == other.CompanySelectionAndCreationDisabledDuringRegistration
                && ShowUnapplicableSeats == other.ShowUnapplicableSeats
                && AllowClassRegistrationFields == other.AllowClassRegistrationFields
                && RegisterEmployeesSearchRequirement == other.RegisterEmployeesSearchRequirement;
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
