using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TActionFilter : Filter
    {
        public Guid? ExcludeActionIdentifier { get; set; }

        public string ActionList { get; set; }
        public string ActionName { get; set; }
        public string ActionUrl { get; set; }
        public string ActionType { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorizationRequirement { get; set; }
        public string ControllerPath { get; set; }
        public string ExtraBreadcrumb { get; set; }

        public bool? HasHelpUrl { get; set; }

        public Guid? NavigationParentActionIdentifier { get; set; }
        public Guid? PermissionParentActionIdentifier { get; set; }

        public TActionFilter Clone()
        {
            return (TActionFilter)MemberwiseClone();
        }
    }
}