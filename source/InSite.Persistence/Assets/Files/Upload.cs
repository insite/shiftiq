using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class Upload
    {
        public Guid ContainerIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid Uploader { get; set; }
        public Guid UploadIdentifier { get; set; }
        public String UploadPrivacyScope { get; set; }
        public String ContainerType { get; set; }
        public String ContentFingerprint { get; set; }
        public String ContentType { get; set; }
        public String Description { get; set; }
        public String Name { get; set; }
        public String NavigateUrl { get; set; }
        public String Title { get; set; }
        public String UploadType { get; set; }
        public Int32? ContentSize { get; set; }
        public DateTimeOffset Uploaded { get; set; }

        public virtual ICollection<UploadRelation> Relationships { get; set; }

        public Upload()
        {
            Relationships = new HashSet<UploadRelation>();
        }
    }
}
