using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class OccupationFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string JobTitle { get; set; }
        public string Keyword { get; set; }
    }
}
