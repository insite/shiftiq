using System;

namespace InSite.Persistence
{
    public class GroupToolkitPermissionSummary
    {
        public bool AllowDelete { get; set; }
        public bool AllowFullControl { get; set; }
        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        
        public Guid GroupIdentifier { get; set; }
        public int ToolkitNumber { get; set; }

        public string GroupName { get; set; }
        public string ToolkitName { get; set; }
    }
}
