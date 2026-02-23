using System;
using System.Collections.Generic;

namespace InSite.Application.Surveys.Read
{
    public class QSurveyOptionItem
    {
        public Guid? BranchToQuestionIdentifier { get; set; }
        public Guid SurveyOptionItemIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }

        public virtual QSurveyQuestion BranchToQuestion { get; set; }
        public virtual QSurveyOptionList SurveyOptionList { get; set; }

        public virtual ICollection<QResponseOption> QResponseOptions { get; set; }
        public virtual ICollection<QSurveyCondition> QSurveyConditions { get; set; }

        public QSurveyOptionItem()
        {
            QResponseOptions = new HashSet<QResponseOption>();
            QSurveyConditions = new HashSet<QSurveyCondition>();
        }
    }
}
