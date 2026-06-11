using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class EmployeeCertificateFilter : Filter
    {
        public Guid? UserIdentifier { get; set; }
        public String EmployeeName { get; set; }
        public String ProfileTitle { get; set; }
    }
}
