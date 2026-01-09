
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CertificateLayoutChanged : Change
    {
        public string Code { get; set; }

        public CertificateLayoutChanged(string code)
        {
            Code = code;
        }
    }
}
