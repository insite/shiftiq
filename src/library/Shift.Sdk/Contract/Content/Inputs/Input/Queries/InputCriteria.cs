using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IInputCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? ContainerId { get; set; }
        Guid[] ContainerIds { get; set; }
        Guid? OrganizationId { get; set; }

        string ContainerType { get; set; }
        string ContentHtml { get; set; }
        string ContentLabel { get; set; }
        string ContentLanguage { get; set; }
        string ContentSnip { get; set; }
        string ContentText { get; set; }
        string ReferenceFiles { get; set; }

        int? ContentSequence { get; set; }
        int? ReferenceCount { get; set; }
    }
}