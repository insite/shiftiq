using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TRubricFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string RubricTitle { get; set; }
        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }

        public TRubricFilter Clone()
        {
            return (TRubricFilter)MemberwiseClone();
        }
    }
}
