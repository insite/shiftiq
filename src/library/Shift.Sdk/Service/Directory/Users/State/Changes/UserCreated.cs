using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserCreated : Change
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }
        public string TimeZone { get; set; }

        public UserCreated(string email, string firstName, string lastName, string middleName, string fullName, string timeZone)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            FullName = fullName;
            TimeZone = timeZone;
        }
    }
}
