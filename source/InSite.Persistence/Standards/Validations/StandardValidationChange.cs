using System;

namespace InSite.Persistence
{
    public class StandardValidationChange
    {
        public Guid StandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid? AuthorUserIdentifier { get; set; }
        public string ChangeComment { get; set; }
        public Guid ChangeIdentifier { get; set; }
        public DateTimeOffset ChangePosted { get; set; }
        public string ChangeStatus { get; set; }

        public Standard Standard { get; set; }
        public User User { get; set; }
        public User Author { get; set; }
    }
}
