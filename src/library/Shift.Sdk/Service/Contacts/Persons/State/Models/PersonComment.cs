using System;

namespace InSite.Domain.Contacts
{
    public class PersonComment
    {
        public Guid Comment { get; set; }
        public Guid Author { get; set; }
        public Guid? Topic { get; set; }
        public Guid Container { get; set; }
        public Guid Organization { get; set; }
        public Guid? ModifiedBy { get; set; }

        public string AuthorName { get; set; }
        public string SubjectType { get; set; }
        public string Text { get; set; }
        public string ContainerType { get; set; }
        public string Flag { get; set; }

        public DateTimeOffset Posted { get; set; }
        public DateTimeOffset? Revised { get; set; }
        public DateTimeOffset? Resolved { get; set; }

        public bool IsPrivate { get; set; }

        public PersonComment Clone()
        {
            return (PersonComment)MemberwiseClone();
        }
    }
}
