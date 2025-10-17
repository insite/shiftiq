using System.Collections.Generic;

namespace Shift.Common
{
    public class PermissionBundle
    {
        public List<string> Policies { get; set; }
        public List<string> Roles { get; set; }

        public PermissionBundle()
        {
            Policies = new List<string>();
            Roles = new List<string>();
        }
    }
}
