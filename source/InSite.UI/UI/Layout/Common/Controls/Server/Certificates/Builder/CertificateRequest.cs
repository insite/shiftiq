using System.Collections.Generic;

namespace InSite.Common.Web.UI.Certificates.Builder
{
    public class CertificateRequest
    {
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string ImagePath { get; set; }
        
        public CertificateBaseElement[] Elements { get; set; }
        public Dictionary<string, string> Variables { get; set; }

        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
    }
}