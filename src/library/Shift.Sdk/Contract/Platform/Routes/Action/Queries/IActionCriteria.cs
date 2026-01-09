using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IActionCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? NavigationParentActionIdentifier { get; set; }

        Guid? PermissionParentActionIdentifier { get; set; }

        string ActionIcon { get; set; }

        string ActionList { get; set; }

        string ActionName { get; set; }

        string ActionNameShort { get; set; }

        string ActionType { get; set; }

        string ActionUrl { get; set; }

        string AuthorityType { get; set; }

        string AuthorizationRequirement { get; set; }

        string ControllerPath { get; set; }

        string ExtraBreadcrumb { get; set; }

        string HelpUrl { get; set; }
    }
}