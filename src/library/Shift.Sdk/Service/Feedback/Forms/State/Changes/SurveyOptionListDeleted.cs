using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionListDeleted : Change
    {
        public SurveyOptionListDeleted(Guid list)
        {
            List = list;
        }

        public Guid List { get; }
    }
}