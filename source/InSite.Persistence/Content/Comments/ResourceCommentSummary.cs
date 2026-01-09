using System;

namespace InSite.Persistence
{
    public class ResourceCommentSummary
    {
        public Int32 AssetNumber { get; set; }
        public String AuthorFirstName { get; set; }
        public String AuthorLastName { get; set; }
        public String AuthorName { get; set; }
        public Guid AuthorTenantIdentifier { get; set; }
        public Guid AuthorUserIdentifier { get; set; }
        public String CommentBody { get; set; }
        public Int32 CommentKey { get; set; }
        public String CommentTitle { get; set; }
        public Boolean IsPublic { get; set; }
        public DateTimeOffset Posted { get; set; }
        public Guid ResourceIdentifier { get; set; }
        public Guid ResourceTenantIdentifier { get; set; }
        public String ResourceTitle { get; set; }
        public String ResourceType { get; set; }
    }
}
