using System;

namespace Shift.Contract
{
    public class ModifyInput
    {
        public Guid ContainerId { get; set; }
        public Guid ContentId { get; set; }
        public Guid OrganizationId { get; set; }

        public string ContainerType { get; set; }
        public string ContentHtml { get; set; }
        public string ContentLabel { get; set; }
        public string ContentLanguage { get; set; }
        public string ContentSnip { get; set; }
        public string ContentText { get; set; }
        public string ReferenceFiles { get; set; }

        public int? ContentSequence { get; set; }
        public int? ReferenceCount { get; set; }
    }
}