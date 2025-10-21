using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TAction
    {
        public Guid ActionIdentifier { get; set; }
        public Guid? NavigationParentActionIdentifier { get; set; }
        public Guid? PermissionParentActionIdentifier { get; set; }

        public string ActionCategory { get; set; }
        public string ActionIcon { get; set; }
        public string ActionList { get; set; }
        public string ActionName { get; set; }
        public string ActionNameShort { get; set; }
        public string ActionType { get; set; }
        public string ActionUrl { get; set; }
        public string ControllerPath { get; set; }
        public string HelpUrl { get; set; }
        public string AuthorizationRequirement { get; set; }

        public bool? AllowUnauthenticatedUsers { get; set; }
        public bool IsToolkitHomePage => ActionUrl != null && ActionUrl.StartsWith("ui/admin/") && ActionUrl.EndsWith("/home") && ActionUrl != "ui/admin/home";

        public virtual TAction NavigationParent { get; set; }
        public virtual TAction PermissionParent { get; set; }
        
        public virtual ICollection<TAction> NavigationChildren { get; set; }
        public virtual ICollection<TAction> PermissionChildren { get; set; }

        public string HelpStatus => !string.IsNullOrEmpty(HelpUrl) ? "Linked" : "Not Linked";
        
        public TAction()
        {
            NavigationChildren = new HashSet<TAction>();
            PermissionChildren = new HashSet<TAction>();
        }
    }
}