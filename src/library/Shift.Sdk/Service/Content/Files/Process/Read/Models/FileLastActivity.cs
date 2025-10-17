using System;

namespace InSite.Application.Files.Read
{
    public class FileLastActivity
    {
        public DateTimeOffset? LastActivityTime { get; set; }
        public Guid? LastActivityUserIdentifier { get; set; }

        public void CopyToModel(FileStorageModel model)
        {
            model.LastActivityTime = LastActivityTime;
            model.LastActivityUserIdentifier = LastActivityUserIdentifier;
        }
    }
}
