using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class LabelFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string LabelName { get; set; }
        public string LabelTranslation { get; set; }
    }
}