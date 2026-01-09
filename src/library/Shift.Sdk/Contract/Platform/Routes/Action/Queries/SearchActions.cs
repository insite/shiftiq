using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchActions : Query<IEnumerable<ActionMatch>>, IActionCriteria
    {
        public Guid? NavigationParentActionIdentifier { get; set; }
        public Guid? PermissionParentActionIdentifier { get; set; }

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