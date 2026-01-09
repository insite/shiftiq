using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserNameModified : Change
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; set; }

        public UserNameModified(string firstName, string lastName, string middleName, string fullName)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            FullName = fullName;
        }
    }
}
