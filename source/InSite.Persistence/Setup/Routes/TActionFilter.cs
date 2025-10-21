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
        
        public string IncludeActionType { get; set; }
        public string ExcludeActionType { get; set; }
        public string ControllerPath { get; set; }

        public bool? HasHelpUrl { get; set; }

        public Guid? NavigationParentActionIdentifier { get; set; }
        public Guid? PermissionParentActionIdentifier { get; set; }

        public TActionFilter Clone()
        {
            return (TActionFilter)MemberwiseClone();
        }
    }
}