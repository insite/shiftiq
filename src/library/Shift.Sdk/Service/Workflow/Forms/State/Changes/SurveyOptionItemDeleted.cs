using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionItemDeleted : Change
    {
        public SurveyOptionItemDeleted(Guid item)
        {
            Item = item;
        }

        public Guid Item { get; }
    }
}