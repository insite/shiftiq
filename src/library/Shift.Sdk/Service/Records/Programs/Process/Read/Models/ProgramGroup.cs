using System;

namespace InSite.Application.Records.Read
{
    public class ProgramGroup
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public int GroupSize { get; set; }
        public DateTimeOffset Added { get; set; }
    }
}
