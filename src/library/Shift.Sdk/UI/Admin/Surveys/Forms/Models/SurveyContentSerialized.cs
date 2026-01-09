using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class SurveyContentSerialized
    {
        public string Language { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
    }
}