using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class CreateUser : Command
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullNamePolicy { get; set; }
        public string TimeZone { get; set; }
        public bool MultiFactorAuthentication { get; set; }

        public CreateUser(Guid userId, string email, string firstName, string lastName, string middleName, string fullNamePolicy, string timeZone, bool multiFactorAuthentication)
        {
            AggregateIdentifier = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            FullNamePolicy = fullNamePolicy;
            TimeZone = timeZone;
            MultiFactorAuthentication = multiFactorAuthentication;
        }
    }
}
