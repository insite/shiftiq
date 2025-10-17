using System;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QExamsWrittenByType
    {
        public int CofQ { get; set; }
        public int IPSE { get; set; }
        public int LevelsFoundationCompletion { get; set; }
        public int Classes { get; set; }
        public int SittingsIndividuals { get; set; }
    }
}
