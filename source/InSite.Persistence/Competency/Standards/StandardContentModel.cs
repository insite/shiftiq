using System;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardContentModel
    {
        public string Language { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
    }
}
