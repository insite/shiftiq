using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Contents.Read
{
    public class TPrivacyUser
    {
        public Guid ContainerIdentifier { get; set; }
        public Guid PrivacyIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ContainerType { get; set; }

        public virtual VUser User { get; set; }
    }
}