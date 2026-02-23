using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyOptionItemSettings : Command
    {
        public ChangeSurveyOptionItemSettings(Guid form, Guid item, string category, decimal points)
        {
            AggregateIdentifier = form;
            Item = item;
            Category = category;
            Points = points;
        }

        public Guid Item { get; }
        public string Category { get; }
        public decimal Points { get; }
    }
}