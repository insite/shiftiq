using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class UserRegistrationFieldMask
    {
        public bool Company { get; set; }
        public bool IsEmpty => !Company;

        public bool IsEqual(UserRegistrationFieldMask other)
        {
            return Company == other.Company;
        }

        public UserRegistrationFieldMask Clone()
        {
            return new UserRegistrationFieldMask
            {
                Company = Company,
            };
        }
    }
}