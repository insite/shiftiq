using System;

namespace InSite.Domain.Records
{
    [Serializable]
    public class Comment
    {
        public Guid Identifier { get; set; }
        public Guid Author { get; set; }
        public Guid Subject { get; set; }
        public string SubjectType { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Posted { get; set; }
        public DateTimeOffset? Revised { get; set; }
        public bool IsPrivate { get; set; }
    }
}
