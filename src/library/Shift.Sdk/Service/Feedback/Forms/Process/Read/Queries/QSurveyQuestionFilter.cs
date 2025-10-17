using System;

using Shift.Common;

using Shift.Constant;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QSurveyQuestionFilter : Filter
    {
        public Guid? SurveyFormIdentifier { get; set; }
        public bool? HasResponseAnswer { get; set; }
        public Guid[] ExcludeQuestionsID { get; set; }
        public SurveyQuestionType[] ExcludeQuestionsTypes { get; set; }
        public SurveyQuestionType[] IncludeQuestionsTypes { get; set; }
        public bool? HasOptions { get; set; }
    }
}
