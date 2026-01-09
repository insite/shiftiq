using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ModifyUserName : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullNamePolicy { get; set; }

        public ModifyUserName(Guid userId, string firstName, string lastName, string middleName, string fullNamePolicy)
        {
            AggregateIdentifier = userId;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            FullNamePolicy = fullNamePolicy;
        }
    }
}
