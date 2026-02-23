using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionItemAdded : Change
    {
        public SurveyOptionItemAdded(Guid list, Guid item)
        {
            List = list;
            Item = item;
        }

        public Guid List { get; }
        public Guid Item { get; }
    }
}