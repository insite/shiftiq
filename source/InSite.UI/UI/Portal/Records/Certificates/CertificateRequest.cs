using System.Collections.Generic;

namespace InSite.UI.Portal.Records.Certificates
{
    public class CertificateRequest
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string ImagePath { get; set; }
        public CertificateBaseElement[] Elements { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}