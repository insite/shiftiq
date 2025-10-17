using System;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QSurveyConditionFilter : Filter
    {
        public Guid? SurveyFormIdentifier { get; set; }
    }
}
