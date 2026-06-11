using System;

namespace InSite.Persistence
{
    public class UploadRelation
    {
        public Guid ContainerIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
        public String ContainerType { get; set; }

        public virtual Upload Upload { get; set; }
    }
}
