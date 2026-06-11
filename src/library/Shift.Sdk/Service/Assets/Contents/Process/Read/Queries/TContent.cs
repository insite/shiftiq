using System;

using Shift.Common;
namespace InSite.Application.Contents.Read
{
    public class TContent
    {
        public TContent() { ContentLanguage = Language.Default; }

        public Guid ContainerIdentifier { get; set; }
        public Guid ContentIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string ContainerType { get; set; }
        public string ContentHtml { get; set; }
        public string ContentLabel { get; set; }
        public string ContentLanguage { get; set; }
        public string ContentSnip { get; set; }
        public string ContentText { get; set; }

        public int? ContentSequence { get; set; }

        public string ReferenceFiles { get; set; }
        public int? ReferenceCount { get; set; }

        public TContent Clone()
        {
            var clone = new TContent();

            this.ShallowCopyTo(clone);

            return clone;
        }
    }
}