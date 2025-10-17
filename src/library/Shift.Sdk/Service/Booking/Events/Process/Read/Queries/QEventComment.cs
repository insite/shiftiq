using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Events.Read
{
    public class QEventComment
    {
        public Guid EventIdentifier { get; set; }
        public Guid AuthorIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        
        public String CommentText { get; set; }

        public DateTimeOffset CommentPosted { get; set; }

        public virtual VUser Author { get; set; }
    }
}
