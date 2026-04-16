using System;

using Shift.Constant;

namespace Shift.Contract
{
    public class WorkshopComment
    {
        public Guid CommentId { get; set; }
        public Guid EntityId { get; set; }
        public string AuthorName { get; set; }
        public string PostedOn { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public FlagType Flag { get; set; }
        public string EventFormat { get; set; }
        public bool IsHidden { get; set; }
    }
}
