using System;

using Shift.Common;

namespace InSite.Application.Surveys2.Read
{
    [Serializable]
    public class VSurveyRespondentFilter : Filter
    {
        public Guid? SurveyFormIdentifier { get; set; }
        public string Keyword { get; set; }
    }
}
