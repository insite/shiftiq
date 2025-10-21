using System;

namespace InSite.Persistence
{
    public class TActionSearchItem
    {
        public string PermissionParentName { get; set; }
        public int PermissionChildCount { get; set; }
        public string NavigationParentName { get; set; }
        public int NavigationChildCount { get; set; }
        public object GroupCount { get; set; }
        public string HelpUrl { get; set; }
        public string ActionType { get; set; }
        public string ControllerPath { get; set; }
        public string ActionUrl { get; set; }
        public string ActionName { get; set; }
        public string ActionList { get; set; }
        public Guid ActionIdentifier { get; set; }
        public string ActionIcon { get; set; }
        public string HelpStatus { get; internal set; }
        public Guid? HelpTopicIdentifier { get; set; }
    }
}