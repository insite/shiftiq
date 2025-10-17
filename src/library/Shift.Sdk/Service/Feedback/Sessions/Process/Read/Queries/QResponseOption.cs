using System;

namespace InSite.Application.Surveys.Read
{
    public class QResponseOption
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyOptionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
        public int OptionSequence { get; set; }

        public bool ResponseOptionIsSelected { get; set; }

        public virtual QResponseSession ResponseSession { get; set; }
        public virtual QSurveyOptionItem SurveyOptionItem { get; set; }
    }
}
