using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Contents.Read
{
    public class TPrivacyGroup
    {
        public Guid ContainerIdentifier { get; set; }
        public Guid? GrantedBy { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid PrivacyIdentifier { get; set; }

        public string ContainerType { get; set; }

        public DateTimeOffset? Granted { get; set; }

        public virtual VGroup Group { get; set; }
    }
}