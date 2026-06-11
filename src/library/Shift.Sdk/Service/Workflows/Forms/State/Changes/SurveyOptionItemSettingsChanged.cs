using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionItemSettingsChanged : Change
    {
        public SurveyOptionItemSettingsChanged(Guid item, string category, decimal points)
        {
            Item = item;
            Category = category;
            Points = points;
        }

        public Guid Item { get; }
        public string Category { get; }
        public decimal Points { get; }
    }
}