using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionListAdded : Change
    {
        public SurveyOptionListAdded(Guid question, Guid list)
        {
            Question = question;
            List = list;
        }

        public Guid Question { get; set; }
        public Guid List { get; }
    }
}