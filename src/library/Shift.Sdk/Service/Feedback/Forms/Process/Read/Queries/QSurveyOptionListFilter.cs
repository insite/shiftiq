using System;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QSurveyOptionListFilter : Filter
    {
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }
    }
}
