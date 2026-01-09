using System;

namespace InSite.Application.Files.Read
{
    [Serializable]
    public class FileProperties
    {
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? Expiry { get; set; }
        public DateTimeOffset? Received { get; set; }
        public DateTimeOffset? Alternated { get; set; }
        public DateTimeOffset? ReviewedTime { get; set; }
        public Guid? ReviewedUserIdentifier { get; set; }
        public DateTimeOffset? ApprovedTime { get; set; }
        public Guid? ApprovedUserIdentifier { get; set; }
        public bool AllowLearnerToView { get; set; } = true;

        public FileProperties Clone() => (FileProperties)MemberwiseClone();
    }
}
