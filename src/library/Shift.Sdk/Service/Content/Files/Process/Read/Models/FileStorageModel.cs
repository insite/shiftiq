using System;
using System.Collections.Generic;

namespace InSite.Application.Files.Read
{
    [Serializable]
    public class FileStorageModel
    {
        public Guid FileIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public FileObjectType ObjectType { get; set; }
        public FileLocation FileLocation { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset Uploaded { get; set; }
        public DateTimeOffset? LastActivityTime { get; set; }
        public Guid? LastActivityUserIdentifier { get; set; }

        public FileProperties Properties { get; set; }
        public IEnumerable<FileClaim> Claims { get; set; }

        public FileStorageModel Clone()
        {
            var clone = (FileStorageModel)MemberwiseClone();

            clone.Properties = Properties?.Clone();
            clone.Claims = FileClaim.CloneList(Claims);

            return clone;
        }
    }
}
