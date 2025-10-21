using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TGroupActionFilter : Filter
    {
        public Guid? ActionIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public bool? AllowRead { get; set; }
        public bool? AllowWrite { get; set; }
        public bool? AllowDelete { get; set; }
        public bool? AllowFullControl { get; set; }

        public string GroupType { get; set; }
    }
}
