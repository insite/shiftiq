using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormLanguagesChanged : Change
    {
        public SurveyFormLanguagesChanged(string language, string[] translations)
        {
            Language = language;
            Translations = translations;
        }

        public string Language { get; }
        public string[] Translations { get; }
    }
}