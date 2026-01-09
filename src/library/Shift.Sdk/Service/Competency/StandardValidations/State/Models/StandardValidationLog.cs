using System;

namespace InSite.Domain.Standards
{
    public class StandardValidationLog
    {
        public Guid LogId { get; set; }
        public Guid? AuthorUserId { get; set; }
        public DateTimeOffset? Posted { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }

        public StandardValidationLog()
        {

        }

        public StandardValidationLog(Guid logId, string status, string comment)
        {
            LogId = logId;
            AuthorUserId = null;
            Posted = null;
            Status = status;
            Comment = comment;
        }

        public StandardValidationLog(Guid logId, Guid? authorUserId, DateTimeOffset? posted, string status, string comment)
            : this(logId, status, comment)
        {
            AuthorUserId = authorUserId;
            Posted = posted;
        }

        public StandardValidationLog Clone()
        {
            return (StandardValidationLog)MemberwiseClone();
        }

        public void Set(StandardValidationLog log)
        {
            AuthorUserId = log.AuthorUserId;
            Posted = log.Posted;
            Status = log.Status;
            Comment = log.Comment;
        }
    }
}
