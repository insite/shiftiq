using System;

namespace Shift.Contract
{
    public partial class ActionModel
    {
        public Guid ActionId { get; set; }
        public Guid? NavigationParentActionId { get; set; }

        public string PermissionParentActionUrl { get; set; }

        public string ActionIcon { get; set; }
        public string ActionList { get; set; }
        public string ActionName { get; set; }
        public string ActionNameShort { get; set; }
        public string ActionType { get; set; }
        public string ActionUrl { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorizationRequirement { get; set; }
        public string ControllerPath { get; set; }
        public string ExtraBreadcrumb { get; set; }
        public string HelpUrl { get; set; }
    }
}