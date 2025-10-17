using System;

namespace InSite.Application.Files.Read
{
    public class OrphanFile
    {
        public Guid FileIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }
        public string ObjectType { get; set; }
    }
}
