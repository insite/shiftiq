using System;

namespace InSite.Application.Files.Read
{
    public class FileActivity
    {
        public Guid FileIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset ActivityTime { get; set; }
        public FileChange[] ActivityChanges { get; set; }
    }
}
