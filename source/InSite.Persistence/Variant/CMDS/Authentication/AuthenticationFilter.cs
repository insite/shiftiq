using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class AuthenticationFilter : Filter
    {
        public string UserEmail { get; set; }
        public DateTime? SinceDate { get; set; }
        public DateTime? BeforeDate { get; set; }
        public bool? SessionIsAuthenticated { get; set; }
    }
}
