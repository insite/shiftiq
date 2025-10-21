using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TContentFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string ContainerIdentifier { get; set; }
        public string ContentLabel { get; set; }
        public string ContentLanguage { get; set; }
        public string Keyword { get; set; }
        public string ContainerType { get; set; }
        public string TextHTML { get; set; }
    }
}
