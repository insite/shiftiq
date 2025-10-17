using System;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QSurveyRespondentFilter : Filter
    {
        public Guid? RespondentUserIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
    }
}
