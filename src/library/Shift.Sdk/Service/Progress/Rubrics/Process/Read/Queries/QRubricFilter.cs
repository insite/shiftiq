using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QRubricFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string RubricTitle { get; set; }
        public DateTimeOffset? CreatedSince { get; set; }
        public DateTimeOffset? CreatedBefore { get; set; }

        public QRubricFilter Clone()
        {
            return (QRubricFilter)MemberwiseClone();
        }
    }
}
