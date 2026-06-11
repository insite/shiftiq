using System;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventAssessmentFormFilter : Filter
    {
        public Guid EventIdentifier { get; set; }
    }
}
