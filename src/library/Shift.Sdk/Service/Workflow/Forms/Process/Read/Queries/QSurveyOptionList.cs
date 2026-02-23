using System;
using System.Collections.Generic;

namespace InSite.Application.Surveys.Read
{
    public class QSurveyOptionList
    {
        public Guid SurveyQuestionIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }

        public int SurveyOptionListSequence { get; set; }

        public virtual QSurveyQuestion SurveyQuestion { get; set; }

        public virtual ICollection<QSurveyOptionItem> QSurveyOptionItems { get; set; }

        public QSurveyOptionList()
        {
            QSurveyOptionItems = new HashSet<QSurveyOptionItem>();
        }
    }
}
