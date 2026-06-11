using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TCollegeCertificate
    {
        public Guid CertificateIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid ProfileIdentifier { get; set; }

        public string CertificateAuthority { get; set; }
        public string CertificateTitle { get; set; }

        public DateTimeOffset? DateGranted { get; set; }
        public DateTimeOffset? DateRequested { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
    }
}
