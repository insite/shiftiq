using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormLanguages : Command
    {
        public ChangeSurveyFormLanguages(Guid form, string language, string[] translations)
        {
            AggregateIdentifier = form;
            Language = language;
            Translations = translations;
        }

        public string Language { get; }
        public string[] Translations { get; }
    }
}