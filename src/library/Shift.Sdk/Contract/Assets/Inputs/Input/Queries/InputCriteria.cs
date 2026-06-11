using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IInputCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }

        Guid? ContainerId { get; set; }
        Guid[] ContainerIds { get; set; }

        string ContainerType { get; set; }
        string ContentLabel { get; set; }
        string ContentLanguage { get; set; }
    }
}
