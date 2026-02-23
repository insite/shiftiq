using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class RenameSurveyForm : Command
    {
        public RenameSurveyForm(Guid form, string name)
        {
            AggregateIdentifier = form;
            Name = name;
        }

        public string Name { get; set; }
    }
}