using System;

namespace InSite.Persistence
{
    public class TCertificateLayout
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid CertificateLayoutIdentifier { get; set; }

        public string CertificateLayoutCode { get; set; }
        public string CertificateLayoutData { get; set; }
    }
}