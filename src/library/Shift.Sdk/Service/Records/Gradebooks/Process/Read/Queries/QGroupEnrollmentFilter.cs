using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class QGroupEnrollmentFilter : Filter
    {
        public Guid? GradebookIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
    }
}
